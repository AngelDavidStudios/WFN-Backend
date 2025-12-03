import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import type { User, Session } from '@supabase/supabase-js'
import { supabase } from '@/utils/supabase'
import type { UserProfile, Role, ModulePermission, LoginCredentials, ModuleName } from '@/types'

export const useAuthStore = defineStore('auth', () => {
  // State
  const user = ref<User | null>(null)
  const session = ref<Session | null>(null)
  const userProfile = ref<UserProfile | null>(null)
  const userRole = ref<Role | null>(null)
  const permissions = ref<ModulePermission[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  const connectionError = ref<string | null>(null)

  // Getters
  const isAuthenticated = computed(() => !!user.value)
  const isSuperAdmin = computed(() => userRole.value?.name === 'SUPER_ADMIN')
  const isAdmin = computed(() => userRole.value?.name === 'ADMIN' || isSuperAdmin.value)
  const fullName = computed(() => {
    if (!userProfile.value) return ''
    return `${userProfile.value.first_name} ${userProfile.value.last_name}`
  })

  // Check if user can access a module
  const canAccessModule = computed(() => {
    return (moduleName: ModuleName): boolean => {
      if (isSuperAdmin.value) return true
      const permission = permissions.value.find((p) => p.module_name === moduleName)
      return permission?.can_view ?? false
    }
  })

  // Check if user can perform action on a module
  const canPerformAction = computed(() => {
    return (moduleName: ModuleName, action: 'view' | 'create' | 'edit' | 'delete'): boolean => {
      if (isSuperAdmin.value) return true
      const permission = permissions.value.find((p) => p.module_name === moduleName)
      if (!permission) return false
      switch (action) {
        case 'view':
          return permission.can_view
        case 'create':
          return permission.can_create
        case 'edit':
          return permission.can_edit
        case 'delete':
          return permission.can_delete
        default:
          return false
      }
    }
  })

  // Get accessible modules for sidebar
  const accessibleModules = computed(() => {
    if (isSuperAdmin.value) {
      return permissions.value
    }
    return permissions.value.filter((p) => p.can_view)
  })

  // Actions
  async function login(credentials: LoginCredentials): Promise<void> {
    loading.value = true
    error.value = null
    connectionError.value = null

    try {
      const { data, error: authError } = await supabase.auth.signInWithPassword({
        email: credentials.email,
        password: credentials.password,
      })

      if (authError) {
        throw authError
      }

      if (data.user && data.session) {
        user.value = data.user
        session.value = data.session

        // Load user profile and permissions
        await loadUserData(data.user.id)
      }
    } catch (err) {
      if (err instanceof Error) {
        error.value = err.message
        if (err.message.includes('fetch') || err.message.includes('network')) {
          connectionError.value = 'Error de conexión. Verifique su internet.'
        }
      } else {
        error.value = 'Error al iniciar sesión'
      }
      throw err
    } finally {
      loading.value = false
    }
  }

  async function logout(): Promise<void> {
    loading.value = true
    try {
      await supabase.auth.signOut()
      clearAuthState()
    } catch (err) {
      console.error('Error al cerrar sesión:', err)
    } finally {
      loading.value = false
    }
  }

  async function checkSession(): Promise<void> {
    loading.value = true
    connectionError.value = null

    try {
      // First, get the session from local storage (no API call)
      const { data: sessionData, error: sessionError } = await supabase.auth.getSession()

      if (sessionError) {
        if (import.meta.env.DEV) {
          console.warn('Session storage error:', sessionError)
        }
        clearAuthState()
        return
      }

      // If there's no session in storage, no need to continue
      if (!sessionData.session) {
        clearAuthState()
        return
      }

      // If we have a session, validate it with the server
      if (import.meta.env.DEV) {
        console.log('🔐 Validating stored session...')
      }
      session.value = sessionData.session

      // Validate the session by getting the user
      const { data: userData, error: userError } = await supabase.auth.getUser()

      if (userError) {
        if (import.meta.env.DEV) {
          console.warn('Session validation failed:', userError.message)
        }
        // Session is invalid, clear it
        clearAuthState()
        return
      }

      if (userData.user) {
        if (import.meta.env.DEV) {
          console.log('✅ Session valid for:', userData.user.email)
        }
        user.value = userData.user
        await loadUserData(userData.user.id)
      } else {
        clearAuthState()
      }
    } catch (err) {
      if (import.meta.env.DEV) {
        console.error('Session check error:', err)
      }
      // On error, clear state
      clearAuthState()
    } finally {
      loading.value = false
    }
  }

  async function loadUserData(userId: string): Promise<void> {
    try {
      if (import.meta.env.DEV) {
        console.log('📋 Loading user data for:', userId)
      }

      // Load user profile
      const { data: profile, error: profileError } = await supabase
        .from('user_profiles')
        .select('*')
        .eq('id', userId)
        .single()

      if (profileError) {
        console.error('❌ Error loading profile:', profileError)
        return
      }

      userProfile.value = profile

      if (import.meta.env.DEV) {
        console.log('✅ Profile loaded:', profile.first_name, profile.last_name)
      }

      // Load role
      if (profile.role_id) {
        const { data: role, error: roleError } = await supabase
          .from('roles')
          .select('*')
          .eq('id', profile.role_id)
          .single()

        if (!roleError && role) {
          userRole.value = role

          if (import.meta.env.DEV) {
            console.log('✅ Role loaded:', role.name, role.display_name)
          }
        } else {
          console.error('❌ Error loading role:', roleError)
        }

        // Load permissions
        const { data: perms, error: permsError } = await supabase
          .from('role_module_permissions')
          .select(
            `
            *,
            modules (
              name,
              display_name,
              route
            )
          `,
          )
          .eq('role_id', profile.role_id)

        if (!permsError && perms) {
          permissions.value = perms.map((p) => ({
            role_id: p.role_id,
            module_id: p.module_id,
            can_view: p.can_view,
            can_create: p.can_create,
            can_edit: p.can_edit,
            can_delete: p.can_delete,
            module_name: p.modules?.name || '',
            module_display_name: p.modules?.display_name || '',
            module_route: p.modules?.route || '',
          }))

          if (import.meta.env.DEV) {
            console.log('✅ Permissions loaded:', permissions.value.length, 'modules')
            console.log('Accessible modules:', permissions.value.map(p => p.module_name))
          }
        } else {
          console.error('❌ Error loading permissions:', permsError)
        }
      }
    } catch (err) {
      console.error('❌ Error loading user data:', err)
    }
  }

  function clearAuthState(): void {
    user.value = null
    session.value = null
    userProfile.value = null
    userRole.value = null
    permissions.value = []
    error.value = null
  }

  // Setup auth state listener
  function initAuthListener() {
    supabase.auth.onAuthStateChange(async (event, newSession) => {
      if (import.meta.env.DEV) {
        console.log('🔄 Auth state:', event)
      }

      if (event === 'SIGNED_IN' && newSession) {
        user.value = newSession.user
        session.value = newSession
        await loadUserData(newSession.user.id)
      } else if (event === 'SIGNED_OUT') {
        clearAuthState()
      } else if (event === 'TOKEN_REFRESHED' && newSession) {
        session.value = newSession
        if (import.meta.env.DEV) {
          console.log('🔄 Token refreshed')
        }
      }
    })
  }

  // Initialize auth listener
  initAuthListener()

  return {
    // State
    user,
    session,
    userProfile,
    userRole,
    permissions,
    loading,
    error,
    connectionError,
    // Getters
    isAuthenticated,
    isSuperAdmin,
    isAdmin,
    fullName,
    canAccessModule,
    canPerformAction,
    accessibleModules,
    // Actions
    login,
    logout,
    checkSession,
    loadUserData,
    clearAuthState,
  }
})

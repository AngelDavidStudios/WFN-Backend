import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import type { User, Session } from '@supabase/supabase-js'
import { supabase } from '@/utils/supabase'
import type {
  UserProfile,
  Role,
  ModulePermission,
  LoginCredentials,
  ModuleName,
} from '@/types'

export const useAuthStore = defineStore('auth', () => {
  // State
  const user = ref<User | null>(null)
  const session = ref<Session | null>(null)
  const userProfile = ref<UserProfile | null>(null)
  const userRole = ref<Role | null>(null)
  const permissions = ref<ModulePermission[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

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
    try {
      const { data } = await supabase.auth.getSession()

      if (data.session) {
        user.value = data.session.user
        session.value = data.session
        await loadUserData(data.session.user.id)
      } else {
        clearAuthState()
      }
    } catch (err) {
      console.error('Error checking session:', err)
      clearAuthState()
    } finally {
      loading.value = false
    }
  }

  async function loadUserData(userId: string): Promise<void> {
    try {
      // Load user profile
      const { data: profile, error: profileError } = await supabase
        .from('user_profiles')
        .select('*')
        .eq('id', userId)
        .single()

      if (profileError) {
        console.error('Error loading profile:', profileError)
        return
      }

      userProfile.value = profile

      // Load role
      if (profile.role_id) {
        const { data: role, error: roleError } = await supabase
          .from('roles')
          .select('*')
          .eq('id', profile.role_id)
          .single()

        if (!roleError && role) {
          userRole.value = role
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
        }
      }
    } catch (err) {
      console.error('Error loading user data:', err)
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

  // Listen to auth state changes
  supabase.auth.onAuthStateChange(async (event, newSession) => {
    if (event === 'SIGNED_IN' && newSession) {
      user.value = newSession.user
      session.value = newSession
      await loadUserData(newSession.user.id)
    } else if (event === 'SIGNED_OUT') {
      clearAuthState()
    }
  })

  return {
    // State
    user,
    session,
    userProfile,
    userRole,
    permissions,
    loading,
    error,
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

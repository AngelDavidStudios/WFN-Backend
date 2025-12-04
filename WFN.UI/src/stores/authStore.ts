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
      // Timeout global para toda la verificación de sesión
      const sessionTimeout = new Promise((_, reject) => {
        setTimeout(() => reject(new Error('TIMEOUT: Session check took too long')), 6000)
      })

      const checkSessionInternal = async () => {
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

          // Cargar datos del usuario en background (no bloquear)
          loadUserData(userData.user.id).catch(err => {
            console.warn('⚠️ Background user data load failed:', err)
          })
        } else {
          clearAuthState()
        }
      }

      // Ejecutar con timeout
      await Promise.race([checkSessionInternal(), sessionTimeout])
    } catch (err: any) {
      if (import.meta.env.DEV) {
        console.error('Session check error:', err)
      }

      // Si es timeout, continuar de todas formas
      if (err.message?.includes('TIMEOUT')) {
        console.warn('⚠️ Session check timed out. Continuing with limited state.')
      } else {
        // On other errors, clear state
        clearAuthState()
      }
    } finally {
      loading.value = false
    }
  }

  async function loadUserData(userId: string): Promise<void> {
    try {
      if (import.meta.env.DEV) {
        console.log('📋 Loading user data for:', userId)
      }

      // Crear un timeout general para toda la carga de datos
      const loadTimeout = new Promise((_, reject) => {
        setTimeout(() => reject(new Error('TIMEOUT: User data loading took too long (>8s)')), 8000)
      })

      // Función para cargar todos los datos
      const loadAllData = async () => {
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

        // Load role - con timeout individual
        if (profile.role_id) {
          try {
            const roleTimeout = new Promise((_, reject) => {
              setTimeout(() => reject(new Error('TIMEOUT: Role loading')), 3000)
            })

            const rolePromise = supabase
              .from('roles')
              .select('*')
              .eq('id', profile.role_id)
              .single()

            const { data: role, error: roleError } = await Promise.race([
              rolePromise,
              roleTimeout.then(() => ({ data: null, error: { message: 'Timeout loading role', code: 'TIMEOUT' } }))
            ]) as any

            if (!roleError && role) {
              userRole.value = role

              if (import.meta.env.DEV) {
                console.log('✅ Role loaded:', role.name, role.display_name)
              }
            } else {
              console.error('❌ Error loading role:', roleError)
              // Continuar sin rol - la app puede funcionar sin esta info
              userRole.value = null
            }
          } catch (roleErr) {
            console.warn('⚠️ Failed to load role (continuing anyway):', roleErr)
            userRole.value = null
          }

          // Load permissions - con timeout individual
          try {
            const permsTimeout = new Promise((_, reject) => {
              setTimeout(() => reject(new Error('TIMEOUT: Permissions loading')), 3000)
            })

            const permsPromise = supabase
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

            const { data: perms, error: permsError } = await Promise.race([
              permsPromise,
              permsTimeout.then(() => ({ data: null, error: { message: 'Timeout loading permissions', code: 'TIMEOUT' } }))
            ]) as any

            if (!permsError && perms) {
              // Debug: Ver estructura de los datos
              if (import.meta.env.DEV) {
                console.log('🔍 Raw permissions data from Supabase:', perms)
                if (perms.length > 0) {
                  console.log('🔍 First permission object:', perms[0])
                  console.log('🔍 modules property:', perms[0].modules)

                  // Verificar si modules está vacío
                  if (!perms[0].modules || !perms[0].modules.name) {
                    console.error('🔥 PROBLEMA DETECTADO: La propiedad "modules" está vacía!')
                    console.error('📖 Causa probable: Políticas RLS bloquean la lectura de la tabla modules')
                    console.error('📖 Solución: Ejecuta SUPABASE_FIX_MODULES_RLS.sql en Supabase SQL Editor')
                  }
                }
              }

              permissions.value = perms.map((p: any) => ({
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

              // Verificar si los nombres de módulos están vacíos
              const emptyModules = permissions.value.filter(p => !p.module_name).length

              if (import.meta.env.DEV) {
                console.log('✅ Permissions loaded:', permissions.value.length, 'modules')

                if (emptyModules > 0) {
                  console.warn(`⚠️ ${emptyModules} permisos tienen módulos vacíos!`)
                  console.warn('📖 Esto impide que aparezcan en el sidebar')
                  console.warn('📖 Solución: Ejecuta SUPABASE_FIX_MODULES_RLS.sql')
                }

                console.log('Accessible modules:', permissions.value.map(p => p.module_name || '<vacío>'))

                // Debug detallado de permisos
                console.table(
                  permissions.value.map(p => ({
                    'Módulo': p.module_display_name || '<VACÍO - RLS BLOQUEADO>',
                    'Ver': p.can_view ? '✅' : '❌',
                    'Crear': p.can_create ? '✅' : '❌',
                    'Editar': p.can_edit ? '✅' : '❌',
                    'Eliminar': p.can_delete ? '✅' : '❌'
                  }))
                )
              }
            } else {
              console.error('❌ Error loading permissions:', permsError)
              // Continuar sin permisos - la app puede funcionar con permisos por defecto
              permissions.value = []

              if (import.meta.env.DEV) {
                console.warn('⚠️ Usuario sin permisos asignados. Para asignar permisos:')
                console.warn('   1. Ve a Supabase Dashboard → SQL Editor')
                console.warn('   2. Ejecuta: SUPABASE_DIAGNOSTICO_PERMISOS.sql')
                console.warn('   3. Asigna permisos al rol del usuario')
              }
            }
          } catch (permsErr) {
            console.warn('⚠️ Failed to load permissions (continuing anyway):', permsErr)
            permissions.value = []
          }
        }
      }

      // Ejecutar con timeout global
      await Promise.race([loadAllData(), loadTimeout])
    } catch (err: any) {
      console.error('❌ Error loading user data:', err)

      // Si es timeout, notificar pero no bloquear
      if (err.message?.includes('TIMEOUT')) {
        console.warn('⚠️ User data loading timed out. App will continue with limited functionality.')
        console.warn('💡 This is likely due to RLS policies. Check SUPABASE_FIX_ROLES_RLS_INFINITE_RECURSION.sql')
      }

      // No lanzar el error - permitir que la app continúe
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

import { ref } from 'vue'
import { defineStore } from 'pinia'
import { supabase } from '@/utils/supabase'
import type { UserProfile, Role, Module, RoleModulePermission } from '@/types'

export const useAdminStore = defineStore('admin', () => {
  const users = ref<UserProfile[]>([])
  const roles = ref<Role[]>([])
  const modules = ref<Module[]>([])
  const permissions = ref<RoleModulePermission[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchUsers() {
    loading.value = true
    try {
      const { data, error: err } = await supabase
        .from('user_profiles')
        .select(
          `
          *,
          roles (
            id,
            name,
            display_name
          )
        `,
        )
        .order('created_at', { ascending: false })

      if (err) throw err
      users.value = data
    } catch (err: any) {
      console.error('Error fetching users:', err)
      error.value = err.message
    } finally {
      loading.value = false
    }
  }

  async function fetchRoles() {
    loading.value = true
    console.log('📋 Fetching roles from Supabase...')

    try {
      // Crear una promesa con timeout para evitar carga infinita
      const timeoutPromise = new Promise((_, reject) => {
        setTimeout(() => reject(new Error('Timeout: La consulta de roles tardó más de 10 segundos')), 10000)
      })

      const fetchPromise = supabase.from('roles').select('*').order('name')

      // Ejecutar con timeout
      const { data, error: err } = await Promise.race([
        fetchPromise,
        timeoutPromise.then(() => ({ data: null, error: { message: 'Timeout', code: 'TIMEOUT' } }))
      ]) as any

      if (err) {
        console.error('❌ Error fetching roles:', err)

        // Detectar errores específicos de RLS
        if (err.code === '42P17' || err.message?.includes('infinite recursion')) {
          console.error('🔥 INFINITE RECURSION DETECTED in RLS policies!')
          console.error('📖 Solution: Execute the following in Supabase SQL Editor:')
          console.error('   DROP POLICY IF EXISTS "Anyone can view roles" ON public.roles;')
          console.error('   CREATE POLICY "Anyone can view roles" ON public.roles FOR SELECT TO authenticated USING (true);')
          error.value = 'Recursión infinita detectada. Las políticas RLS de la tabla roles están mal configuradas.'
          roles.value = [] // Establecer array vacío en lugar de dejar undefined
          return
        } else if (err.code === 'TIMEOUT') {
          console.error('⏱️ TIMEOUT: Query took too long (>10s). Likely RLS infinite recursion.')
          error.value = 'La consulta tardó demasiado. Verifica las políticas RLS de la tabla roles.'
          roles.value = []
          return
        } else if (err.code === '42501') {
          console.error('🔒 PERMISSION DENIED: Check RLS policies for roles table')
          error.value = 'Permisos insuficientes. Verifica las políticas RLS de la tabla roles.'
          roles.value = []
          return
        } else {
          error.value = err.message
          roles.value = []
          return
        }
      }

      console.log('✅ Roles fetched successfully:', data?.length || 0, 'roles')
      console.log('Roles data:', data)
      roles.value = data || []

      if (data?.length === 0) {
        console.warn('⚠️ No roles found in database. Create roles first in "Roles y Permisos" tab.')
      }
    } catch (err: any) {
      console.error('❌ Unexpected error fetching roles:', err)
      roles.value = []
      error.value = err.message || 'Error desconocido al cargar roles'
    } finally {
      loading.value = false
      console.log('🏁 fetchRoles completed. Loading state:', loading.value, 'Roles count:', roles.value.length)
    }
  }

  async function fetchModules() {
    loading.value = true
    try {
      const { data, error: err } = await supabase.from('modules').select('*').order('display_name')

      if (err) throw err
      modules.value = data
    } catch (err: any) {
      console.error('Error fetching modules:', err)
      error.value = err.message
    } finally {
      loading.value = false
    }
  }

  async function fetchRolePermissions(roleId: string) {
    loading.value = true
    try {
      const { data, error: err } = await supabase
        .from('role_module_permissions')
        .select(
          `
          *,
          modules (
            id,
            name,
            display_name
          )
        `,
        )
        .eq('role_id', roleId)

      if (err) throw err
      permissions.value = data
    } catch (err: any) {
      console.error('Error fetching permissions:', err)
      error.value = err.message
    } finally {
      loading.value = false
    }
  }

  async function updateUserRole(userId: string, roleId: string) {
    loading.value = true
    try {
      const { error: err } = await supabase
        .from('user_profiles')
        .update({ role_id: roleId })
        .eq('id', userId)

      if (err) throw err

      // Update local state
      const userIndex = users.value.findIndex((u) => u.id === userId)
      if (userIndex !== -1) {
        users.value[userIndex].role_id = roleId
        // We might need to refresh the user list or manually update the role object if needed
        await fetchUsers()
      }
    } catch (err: any) {
      console.error('Error updating user role:', err)
      error.value = err.message
      throw err
    } finally {
      loading.value = false
    }
  }

  async function createRole(role: Partial<Role>) {
    loading.value = true
    try {
      const { data, error: err } = await supabase.from('roles').insert(role).select().single()

      if (err) throw err
      roles.value.push(data)
      return data
    } catch (err: any) {
      console.error('Error creating role:', err)
      error.value = err.message
      throw err
    } finally {
      loading.value = false
    }
  }

  async function createUser(userData: any) {
    loading.value = true
    console.log('📝 Creating user with data:', { ...userData, password: '***' })

    try {
      // Nota: Dado que no tenemos service_role key en el frontend (por seguridad),
      // usamos signUp y luego creamos/actualizamos el perfil
      // El usuario recibirá un email de confirmación automáticamente

      const { data: authData, error: authError } = await supabase.auth.signUp({
        email: userData.email,
        password: userData.password,
        options: {
          data: {
            first_name: userData.first_name,
            last_name: userData.last_name,
            role_id: userData.role_id,
          }
        }
      })

      if (authError) {
        console.error('❌ Auth error:', authError)
        throw authError
      }

      if (!authData.user) {
        throw new Error('No se pudo crear el usuario')
      }

      console.log('✅ User created in Auth:', authData.user.id)

      // 2. Crear/actualizar el perfil en user_profiles
      // Nota: El trigger en Supabase debería crear el perfil automáticamente
      // pero lo hacemos explícitamente para asegurar que tenga el role_id correcto
      const { error: profileError } = await supabase
        .from('user_profiles')
        .upsert({
          id: authData.user.id,
          email: userData.email,
          first_name: userData.first_name,
          last_name: userData.last_name,
          role_id: userData.role_id,
        }, {
          onConflict: 'id'
        })

      if (profileError) {
        console.error('❌ Profile error:', profileError)
        throw profileError
      }

      console.log('✅ User profile created/updated successfully')

      // 3. Refrescar la lista de usuarios
      await fetchUsers()

      return authData.user
    } catch (err: any) {
      console.error('❌ Error creating user:', err)
      error.value = err.message || 'Error al crear usuario'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function updateUser(userId: string, updates: Partial<UserProfile>) {
    loading.value = true
    try {
      // For updates we can use direct SQL since we have RLS policies
      const { error: err } = await supabase.from('user_profiles').update(updates).eq('id', userId)

      if (err) throw err

      // Update local state
      const userIndex = users.value.findIndex((u) => u.id === userId)
      if (userIndex !== -1) {
        users.value[userIndex] = { ...users.value[userIndex], ...updates }
        await fetchUsers() // Refresh to get joined role data if needed
      }
    } catch (err: any) {
      console.error('Error updating user:', err)
      error.value = err.message
      throw err
    } finally {
      loading.value = false
    }
  }

  async function deleteUser(userId: string) {
    loading.value = true
    console.log('🗑️ Attempting to delete/deactivate user:', userId)

    try {
      // Estrategia 1: Intentar soft delete (marcar como inactivo)
      // Esto es preferible porque mantiene el historial
      console.log('Strategy 1: Soft delete (update is_active to false)...')

      const { error: softDeleteError } = await supabase
        .from('user_profiles')
        .update({ is_active: false })
        .eq('id', userId)

      if (!softDeleteError) {
        console.log('✅ User deactivated successfully (soft delete)')
        users.value = users.value.filter((u) => u.id !== userId)
        return
      }

      // Si el soft delete falla, intentar eliminación real
      console.warn('⚠️ Soft delete failed, trying hard delete...', softDeleteError)
      console.log('Strategy 2: Hard delete (DELETE from user_profiles)...')

      const { error: hardDeleteError } = await supabase
        .from('user_profiles')
        .delete()
        .eq('id', userId)

      if (!hardDeleteError) {
        console.log('✅ User deleted successfully (hard delete)')
        users.value = users.value.filter((u) => u.id !== userId)
        return
      }

      // Si ambas estrategias fallan, lanzar el error
      console.error('❌ Both delete strategies failed')
      console.error('Soft delete error:', softDeleteError)
      console.error('Hard delete error:', hardDeleteError)

      // Lanzar el error más relevante
      throw hardDeleteError || softDeleteError

    } catch (err: any) {
      console.error('❌ Error deleting/deactivating user:', err)

      // Mensaje de error más descriptivo
      if (err.code === 'PGRST301' || err.message?.includes('CORS')) {
        error.value = 'Error de CORS. Verifica las políticas RLS en Supabase ejecutando SUPABASE_FIX_USER_UPDATE_RLS.sql'
      } else if (err.code === '42501') {
        error.value = 'Sin permisos. Asegúrate de tener rol SUPER_ADMIN y ejecuta SUPABASE_FIX_USER_UPDATE_RLS.sql'
      } else {
        error.value = err.message || 'Error al eliminar usuario'
      }

      throw err
    } finally {
      loading.value = false
    }
  }

  async function updateRolePermissions(
    roleId: string,
    newPermissions: Partial<RoleModulePermission>[],
  ) {
    loading.value = true
    try {
      // We can upsert permissions
      const { error: err } = await supabase.from('role_module_permissions').upsert(
        newPermissions.map((p) => ({
          role_id: roleId,
          module_id: p.module_id,
          can_view: p.can_view,
          can_create: p.can_create,
          can_edit: p.can_edit,
          can_delete: p.can_delete,
        })),
      )

      if (err) throw err
      await fetchRolePermissions(roleId)
    } catch (err: any) {
      console.error('Error updating permissions:', err)
      error.value = err.message
      throw err
    } finally {
      loading.value = false
    }
  }

  return {
    users,
    roles,
    modules,
    permissions,
    loading,
    error,
    fetchUsers,
    fetchRoles,
    fetchModules,
    fetchRolePermissions,
    updateUserRole,
    createRole,
    updateRolePermissions,
    createUser,
    updateUser,
    deleteUser,
  }
})

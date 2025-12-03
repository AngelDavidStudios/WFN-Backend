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
      const { data, error: err } = await supabase.from('roles').select('*').order('name')

      if (err) {
        console.error('❌ Error fetching roles:', err)
        throw err
      }

      console.log('✅ Roles fetched successfully:', data?.length || 0, 'roles')
      console.log('Roles data:', data)
      roles.value = data || []
    } catch (err: any) {
      console.error('❌ Error fetching roles:', err)
      error.value = err.message
      roles.value = []
    } finally {
      loading.value = false
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
    try {
      const { data, error: err } = await supabase.functions.invoke('admin-users', {
        body: {
          action: 'create',
          ...userData,
        },
      })

      if (err) throw err
      if (data.error) throw new Error(data.error)

      await fetchUsers()
      return data
    } catch (err: any) {
      console.error('Error creating user:', err)
      error.value = err.message
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
    try {
      const { data, error: err } = await supabase.functions.invoke('admin-users', {
        body: {
          action: 'delete',
          userId,
        },
      })

      if (err) throw err
      if (data.error) throw new Error(data.error)

      // Remove from local state
      users.value = users.value.filter((u) => u.id !== userId)
    } catch (err: any) {
      console.error('Error deleting user:', err)
      error.value = err.message
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

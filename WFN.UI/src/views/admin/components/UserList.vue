<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useAdminStore } from '@/stores/adminStore'
import { useUIStore } from '@/stores'
import { PlusIcon, PencilIcon, TrashIcon } from '@heroicons/vue/24/outline'
import DataTable from '@/components/tables/DataTable.vue'
import BaseModal from '@/components/common/BaseModal.vue'
import BaseButton from '@/components/common/BaseButton.vue'
import FormInput from '@/components/forms/FormInput.vue'
import FormSelect from '@/components/forms/FormSelect.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import type { UserProfile } from '@/types'

const adminStore = useAdminStore()
const uiStore = useUIStore()

const columns = [
  { key: 'first_name', label: 'Nombre' },
  { key: 'last_name', label: 'Apellido' },
  { key: 'role', label: 'Rol' },
  { key: 'created_at', label: 'Fecha Registro' },
]

const selectedUser = ref<UserProfile | null>(null)

const showUserModal = ref(false)
const isEditing = ref(false)
const showDeleteModal = ref(false)
const userForm = ref({
  id: '',
  email: '',
  password: '',
  first_name: '',
  last_name: '',
  role_id: '',
})

onMounted(async () => {
  console.log('🔄 UserList mounted - fetching users and roles...')
  try {
    await Promise.all([adminStore.fetchUsers(), adminStore.fetchRoles()])
    console.log('✅ Roles loaded:', adminStore.roles.length, adminStore.roles)
    console.log('✅ Users loaded:', adminStore.users.length)

    if (adminStore.roles.length === 0) {
      console.warn('⚠️ WARNING: No roles found. Please check:')
      console.warn('   1. Do roles exist in Supabase? Run: SELECT * FROM roles;')
      console.warn('   2. Are RLS policies configured correctly? Check for infinite recursion errors.')
      console.warn('   3. Is the user authenticated? Check auth session.')
      uiStore.notifyWarning(
        'Sin roles disponibles',
        'No se encontraron roles. Por favor, verifica las políticas RLS en Supabase o crea roles primero.'
      )
    }
  } catch (error: any) {
    console.error('❌ Error loading users or roles:', error)
    uiStore.notifyError('Error', 'No se pudieron cargar los datos. Verifica las políticas RLS en Supabase.')
  }
})

const availableRoles = computed(() => {
  console.log('🔍 Computing available roles:', adminStore.roles.length, adminStore.roles)
  return adminStore.roles
})

const roleOptions = computed(() => {
  const options = adminStore.roles.map((role) => ({
    label: `${role.display_name} (${role.name})`,
    value: role.id,
  }))
  console.log('📋 Role options for dropdown:', options)
  return options
})

const usersWithRoleName = computed(() => {
  return adminStore.users.map((user) => ({
    ...user,
    role: user.roles?.display_name || 'Sin Rol',
  }))
})

function openCreateModal() {
  isEditing.value = false
  userForm.value = {
    id: '',
    email: '',
    password: '',
    first_name: '',
    last_name: '',
    role_id: '',
  }
  showUserModal.value = true
}

function openEditModal(user: UserProfile) {
  isEditing.value = true
  userForm.value = {
    id: user.id,
    email: user.email,
    password: '', // Password not editable directly here usually, but we can leave blank
    first_name: user.first_name,
    last_name: user.last_name,
    role_id: user.role_id,
  }
  showUserModal.value = true
}

function openDeleteModal(user: UserProfile) {
  selectedUser.value = user
  showDeleteModal.value = true
}

async function saveUser() {
  // Validaciones
  if (!userForm.value.email || !userForm.value.email.includes('@')) {
    uiStore.notifyWarning('Validación', 'Por favor ingrese un email válido')
    return
  }

  if (!userForm.value.role_id) {
    uiStore.notifyWarning('Validación', 'Por favor seleccione un rol')
    return
  }

  if (!isEditing.value && (!userForm.value.password || userForm.value.password.length < 6)) {
    uiStore.notifyWarning('Validación', 'La contraseña debe tener al menos 6 caracteres')
    return
  }

  if (!userForm.value.first_name) {
    uiStore.notifyWarning('Validación', 'El nombre es requerido')
    return
  }

  try {
    if (isEditing.value) {
      const { id, ...updates } = userForm.value
      await adminStore.updateUser(id, {
        first_name: updates.first_name,
        last_name: updates.last_name,
        role_id: updates.role_id,
      } as Partial<UserProfile>)

      uiStore.notifySuccess('Éxito', 'Usuario actualizado correctamente')
    } else {
      await adminStore.createUser({
        email: userForm.value.email,
        password: userForm.value.password,
        first_name: userForm.value.first_name,
        last_name: userForm.value.last_name,
        role_id: userForm.value.role_id,
      })

      uiStore.notifySuccess('Éxito', 'Usuario creado correctamente. Se ha enviado un email de confirmación.')
    }

    showUserModal.value = false
    // No necesitamos fetchUsers() aquí porque createUser() y updateUser() ya refrescan la lista
  } catch (error: any) {
    console.error('Error saving user:', error)
    const errorMessage = error.message || 'Error al guardar usuario'
    uiStore.notifyError('Error', errorMessage)
  }
}

async function deleteUser() {
  if (!selectedUser.value) return

  const userEmail = selectedUser.value.email

  try {
    await adminStore.deleteUser(selectedUser.value.id)
    showDeleteModal.value = false
    selectedUser.value = null

    uiStore.notifySuccess('Éxito', 'Usuario eliminado de la base de datos')

    // Notificación adicional recordando eliminar de Auth
    setTimeout(() => {
      uiStore.notifyWarning(
        'Acción Pendiente',
        `Recuerda: Debes eliminar manualmente "${userEmail}" de Supabase Dashboard → Authentication → Users`
      )
    }, 2000)

    // No necesitamos fetchUsers() aquí porque deleteUser() ya actualiza el estado local
  } catch (error: any) {
    console.error('Error deleting user:', error)
    const errorMessage = error.message || 'No se pudo eliminar el usuario'
    uiStore.notifyError('Error', errorMessage)
  }
}

function formatDate(dateString: string) {
  if (!dateString) return ''
  return new Date(dateString).toLocaleDateString()
}
</script>

<template>
  <div class="space-y-6">
    <!-- Error Banner - RLS Issues -->
    <div v-if="adminStore.error && adminStore.error.includes('recursión')" class="bg-red-50 border-l-4 border-red-400 p-4">
      <div class="flex">
        <div class="flex-shrink-0">
          <svg class="h-5 w-5 text-red-400" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
          </svg>
        </div>
        <div class="ml-3">
          <h3 class="text-sm font-medium text-red-800">
            🔥 Error de Recursión Infinita Detectado
          </h3>
          <div class="mt-2 text-sm text-red-700">
            <p class="mb-2">Las políticas RLS de la tabla <code class="bg-red-100 px-1 rounded font-mono">roles</code> están causando recursión infinita.</p>
            <p class="font-semibold">Solución inmediata:</p>
            <ol class="list-decimal list-inside space-y-1 mt-1 ml-2">
              <li>Abre <a href="https://supabase.com/dashboard" target="_blank" class="underline font-semibold">Supabase Dashboard</a></li>
              <li>Ve a <strong>SQL Editor</strong></li>
              <li>Ejecuta el script: <code class="bg-red-100 px-1 rounded font-mono">SUPABASE_FIX_ROLES_RLS_INFINITE_RECURSION.sql</code></li>
              <li>Recarga esta página</li>
            </ol>
          </div>
        </div>
      </div>
    </div>

    <!-- Info Banner sobre Authentication -->
    <div class="bg-blue-50 border-l-4 border-blue-400 p-4">
      <div class="flex">
        <div class="flex-shrink-0">
          <svg class="h-5 w-5 text-blue-400" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd" />
          </svg>
        </div>
        <div class="ml-3">
          <p class="text-sm text-blue-700">
            <strong>Nota importante:</strong> Al eliminar un usuario de esta lista, se elimina de la base de datos pero
            <strong>debes eliminarlo manualmente</strong> de
            <a
              href="https://supabase.com/dashboard"
              target="_blank"
              class="underline hover:text-blue-900"
            >
              Supabase Dashboard → Authentication → Users
            </a>
          </p>
        </div>
      </div>
    </div>

    <!-- Header Actions -->
    <div class="flex items-center justify-between">
      <div>
        <p class="text-sm text-gray-600">Administre los usuarios del sistema y sus roles</p>
      </div>
      <BaseButton variant="primary" @click="openCreateModal">
        <PlusIcon class="h-5 w-5 mr-2" />
        Crear Usuario
      </BaseButton>
    </div>

    <!-- Loading State -->
    <div v-if="adminStore.loading && usersWithRoleName.length === 0" class="flex justify-center py-12">
      <LoadingSpinner size="lg" />
    </div>

    <!-- Users Table -->
    <DataTable
      v-else
      :columns="columns"
      :data="usersWithRoleName"
      :loading="adminStore.loading"
      empty-text="No hay usuarios registrados"
    >
      <template #cell-created_at="{ value }">
        {{ formatDate(value as string) }}
      </template>

      <template #actions="{ row }">
        <div class="flex items-center space-x-2">
          <button
            @click.stop="openEditModal(row as unknown as UserProfile)"
            class="text-gray-400 hover:text-blue-600"
            title="Editar usuario"
          >
            <PencilIcon class="h-5 w-5" />
          </button>
          <button
            @click.stop="openDeleteModal(row as unknown as UserProfile)"
            class="text-gray-400 hover:text-red-600"
            title="Eliminar usuario"
          >
            <TrashIcon class="h-5 w-5" />
          </button>
        </div>
      </template>
    </DataTable>

    <!-- User Modal (Create/Edit) -->
    <BaseModal
      :open="showUserModal"
      @close="showUserModal = false"
      :title="isEditing ? 'Editar Usuario' : 'Crear Usuario'"
    >
      <div class="space-y-4">
        <!-- Email (solo en creación) -->
        <FormInput
          v-if="!isEditing"
          v-model="userForm.email"
          label="Email"
          type="email"
          placeholder="usuario@ejemplo.com"
          required
          help-text="El usuario recibirá un correo de confirmación"
        />

        <!-- Password (solo en creación) -->
        <FormInput
          v-if="!isEditing"
          v-model="userForm.password"
          label="Contraseña"
          type="password"
          placeholder="Mínimo 6 caracteres"
          required
          help-text="Contraseña temporal para el usuario"
        />

        <!-- Nombre y Apellido -->
        <div class="grid grid-cols-2 gap-4">
          <FormInput
            v-model="userForm.first_name"
            label="Nombre"
            type="text"
            placeholder="Juan"
            required
          />
          <FormInput
            v-model="userForm.last_name"
            label="Apellido"
            type="text"
            placeholder="Pérez"
            required
          />
        </div>

        <!-- Rol -->
        <FormSelect
          v-model="userForm.role_id"
          label="Rol"
          :options="roleOptions"
          :placeholder="roleOptions.length > 0
            ? `Seleccionar Rol (${roleOptions.length} disponibles)`
            : '⚠️ No hay roles disponibles - Verifica RLS en Supabase'"
          required
        />

        <!-- Debug info -->
        <div v-if="roleOptions.length === 0" class="bg-yellow-50 border-l-4 border-yellow-400 p-4">
          <div class="flex">
            <div class="flex-shrink-0">
              <svg class="h-5 w-5 text-yellow-400" viewBox="0 0 20 20" fill="currentColor">
                <path fill-rule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clip-rule="evenodd" />
              </svg>
            </div>
            <div class="ml-3">
              <p class="text-sm text-yellow-700">
                <strong>No hay roles disponibles.</strong> Por favor:
              </p>
              <ul class="mt-2 text-xs text-yellow-600 list-disc list-inside space-y-1">
                <li>Verifica que existen roles en Supabase: <code class="bg-yellow-100 px-1 rounded">SELECT * FROM roles;</code></li>
                <li>Verifica las políticas RLS en la tabla <code class="bg-yellow-100 px-1 rounded">roles</code></li>
                <li>Crea al menos un rol en la pestaña "Roles y Permisos"</li>
              </ul>
            </div>
          </div>
        </div>

        <!-- Info banner -->
        <div v-if="!isEditing" class="bg-blue-50 border-l-4 border-blue-400 p-4">
          <div class="flex">
            <div class="flex-shrink-0">
              <svg class="h-5 w-5 text-blue-400" viewBox="0 0 20 20" fill="currentColor">
                <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd" />
              </svg>
            </div>
            <div class="ml-3">
              <p class="text-sm text-blue-700">
                El usuario recibirá un correo electrónico para confirmar su cuenta y establecer su contraseña definitiva.
              </p>
            </div>
          </div>
        </div>
      </div>

      <template #footer>
        <div class="flex justify-end space-x-3">
          <BaseButton variant="outline" @click="showUserModal = false" :disabled="adminStore.loading">
            Cancelar
          </BaseButton>
          <BaseButton variant="primary" @click="saveUser" :loading="adminStore.loading">
            {{ isEditing ? 'Guardar Cambios' : 'Crear Usuario' }}
          </BaseButton>
        </div>
      </template>
    </BaseModal>

    <!-- Delete Confirmation Modal -->
    <BaseModal
      :open="showDeleteModal"
      @close="showDeleteModal = false"
      title="Eliminar Usuario"
    >
      <div class="space-y-4">
        <div class="bg-yellow-50 border-l-4 border-yellow-400 p-4">
          <div class="flex">
            <div class="flex-shrink-0">
              <svg class="h-5 w-5 text-yellow-400" viewBox="0 0 20 20" fill="currentColor">
                <path fill-rule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clip-rule="evenodd" />
              </svg>
            </div>
            <div class="ml-3">
              <h3 class="text-sm font-medium text-yellow-800">
                ¿Está seguro de eliminar este usuario?
              </h3>
            </div>
          </div>
        </div>

        <div v-if="selectedUser" class="space-y-2">
          <p class="text-sm text-gray-700">
            <strong>Usuario:</strong> {{ selectedUser.first_name }} {{ selectedUser.last_name }}
          </p>
          <p class="text-sm text-gray-700">
            <strong>Email:</strong> {{ selectedUser.email }}
          </p>
          <p class="text-sm text-gray-700">
            <strong>Rol:</strong> {{ selectedUser.roles?.display_name || 'Sin Rol' }}
          </p>
        </div>

        <div class="bg-red-50 border border-red-200 rounded-lg p-4">
          <p class="text-sm text-red-800">
            <strong>Advertencia:</strong> Esta acción no se puede deshacer. El usuario perderá acceso al sistema inmediatamente.
          </p>
        </div>
      </div>

      <template #footer>
        <div class="flex justify-end space-x-3">
          <BaseButton variant="outline" @click="showDeleteModal = false" :disabled="adminStore.loading">
            Cancelar
          </BaseButton>
          <BaseButton variant="danger" @click="deleteUser" :loading="adminStore.loading">
            Sí, Eliminar Usuario
          </BaseButton>
        </div>
      </template>
    </BaseModal>
  </div>
</template>

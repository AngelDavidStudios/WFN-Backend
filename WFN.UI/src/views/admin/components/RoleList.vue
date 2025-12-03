<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAdminStore } from '@/stores/adminStore'
import { useUIStore } from '@/stores'
import { PlusIcon, Cog6ToothIcon } from '@heroicons/vue/24/outline'
import DataTable from '@/components/tables/DataTable.vue'
import BaseModal from '@/components/common/BaseModal.vue'
import BaseButton from '@/components/common/BaseButton.vue'
import FormInput from '@/components/forms/FormInput.vue'
import FormTextarea from '@/components/forms/FormTextarea.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import type { Role, RoleModulePermission } from '@/types'

interface ExtendedPermission extends RoleModulePermission {
  module_name?: string
  module_display_name?: string
}

const adminStore = useAdminStore()
const uiStore = useUIStore()

const columns = [
  { key: 'display_name', label: 'Nombre' },
  { key: 'description', label: 'Descripción' },
  { key: 'created_at', label: 'Fecha Creación' },
]

const showPermissionsModal = ref(false)
const selectedRole = ref<Role | null>(null)
const rolePermissions = ref<ExtendedPermission[]>([])

onMounted(async () => {
  await adminStore.fetchRoles()
  await adminStore.fetchModules()
})

const showCreateRoleModal = ref(false)
const newRole = ref({
  name: '',
  display_name: '',
  description: '',
})

async function createRole() {
  // Validaciones
  if (!newRole.value.name || !newRole.value.display_name) {
    uiStore.notifyWarning('Validación', 'El nombre y nombre visible son requeridos')
    return
  }

  try {
    await adminStore.createRole(newRole.value)
    showCreateRoleModal.value = false
    newRole.value = { name: '', display_name: '', description: '' }

    uiStore.notifySuccess('Éxito', 'Rol creado correctamente')
    await adminStore.fetchRoles()
  } catch (error: any) {
    console.error('Error creating role:', error)
    const errorMessage = error.message || 'No se pudo crear el rol'
    uiStore.notifyError('Error', errorMessage)
  }
}

async function openPermissionsModal(role: Role) {
  selectedRole.value = role
  await adminStore.fetchRolePermissions(role.id)

  // Merge with all modules to ensure we have a row for each module
  // If a permission doesn't exist for a module, create a default one (in memory)
  rolePermissions.value = adminStore.modules.map((module) => {
    const existing = adminStore.permissions.find((p) => p.module_id === module.id)
    if (existing) {
      return {
        ...existing,
        module_display_name: module.display_name, // Ensure display name is from module list
      }
    }

    return {
      role_id: role.id,
      module_id: module.id,
      module_name: module.name,
      module_display_name: module.display_name,
      can_view: false,
      can_create: false,
      can_edit: false,
      can_delete: false,
    } as ExtendedPermission
  })

  showPermissionsModal.value = true
}

async function savePermissions() {
  if (!selectedRole.value) return

  try {
    await adminStore.updateRolePermissions(selectedRole.value.id, rolePermissions.value)
    showPermissionsModal.value = false

    uiStore.notifySuccess('Éxito', 'Permisos actualizados correctamente')
  } catch (error: any) {
    console.error('Error saving permissions:', error)
    const errorMessage = error.message || 'No se pudieron guardar los permisos'
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
    <!-- Header Actions -->
    <div class="flex items-center justify-between">
      <div>
        <p class="text-sm text-gray-600">Administre los roles del sistema y sus permisos</p>
      </div>
      <BaseButton variant="primary" @click="showCreateRoleModal = true">
        <PlusIcon class="h-5 w-5 mr-2" />
        Crear Rol
      </BaseButton>
    </div>

    <!-- Loading State -->
    <div v-if="adminStore.loading && adminStore.roles.length === 0" class="flex justify-center py-12">
      <LoadingSpinner size="lg" />
    </div>

    <!-- Roles Table -->
    <DataTable
      v-else
      :columns="columns"
      :data="adminStore.roles"
      :loading="adminStore.loading"
      empty-text="No hay roles registrados"
    >
      <template #cell-created_at="{ value }">
        {{ formatDate(value as string) }}
      </template>

      <template #actions="{ row }">
        <button
          @click.stop="openPermissionsModal(row as unknown as Role)"
          class="inline-flex items-center text-gray-400 hover:text-blue-600"
          title="Gestionar permisos"
        >
          <Cog6ToothIcon class="h-5 w-5 mr-1" />
          <span class="text-sm font-medium">Permisos</span>
        </button>
      </template>
    </DataTable>

    <!-- Create Role Modal -->
    <BaseModal
      :open="showCreateRoleModal"
      @close="showCreateRoleModal = false"
      title="Crear Nuevo Rol"
    >
      <div class="space-y-4">
        <FormInput
          v-model="newRole.name"
          label="Nombre (Código)"
          type="text"
          placeholder="RRHH"
          required
          help-text="Código interno del rol en mayúsculas"
        />

        <FormInput
          v-model="newRole.display_name"
          label="Nombre Visible"
          type="text"
          placeholder="Recursos Humanos"
          required
          help-text="Nombre que se mostrará en la interfaz"
        />

        <FormTextarea
          v-model="newRole.description"
          label="Descripción"
          rows="3"
          placeholder="Describe las responsabilidades de este rol..."
          help-text="Breve descripción del rol y sus funciones"
        />

        <!-- Info banner -->
        <div class="bg-blue-50 border-l-4 border-blue-400 p-4">
          <div class="flex">
            <div class="flex-shrink-0">
              <svg class="h-5 w-5 text-blue-400" viewBox="0 0 20 20" fill="currentColor">
                <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd" />
              </svg>
            </div>
            <div class="ml-3">
              <p class="text-sm text-blue-700">
                Después de crear el rol, podrá asignar permisos específicos para cada módulo del sistema.
              </p>
            </div>
          </div>
        </div>
      </div>

      <template #footer>
        <div class="flex justify-end space-x-3">
          <BaseButton variant="outline" @click="showCreateRoleModal = false" :disabled="adminStore.loading">
            Cancelar
          </BaseButton>
          <BaseButton variant="primary" @click="createRole" :loading="adminStore.loading">
            Crear Rol
          </BaseButton>
        </div>
      </template>
    </BaseModal>

    <!-- Permissions Modal -->
    <BaseModal
      :open="showPermissionsModal"
      @close="showPermissionsModal = false"
      title="Gestionar Permisos"
      size="xl"
    >
      <div class="space-y-4">
        <p class="text-sm text-gray-500">
          Configura los permisos para el rol <strong>{{ selectedRole?.display_name }}</strong>
        </p>

        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-300">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">Módulo</th>
                <th class="px-3 py-3.5 text-center text-sm font-semibold text-gray-900">Ver</th>
                <th class="px-3 py-3.5 text-center text-sm font-semibold text-gray-900">Crear</th>
                <th class="px-3 py-3.5 text-center text-sm font-semibold text-gray-900">Editar</th>
                <th class="px-3 py-3.5 text-center text-sm font-semibold text-gray-900">
                  Eliminar
                </th>
              </tr>
            </thead>
            <tbody class="divide-y divide-gray-200 bg-white">
              <tr v-for="perm in rolePermissions" :key="perm.module_id">
                <td class="whitespace-nowrap px-3 py-4 text-sm text-gray-500">
                  {{ perm.module_display_name || perm.module_name }}
                </td>
                <td class="whitespace-nowrap px-3 py-4 text-sm text-gray-500 text-center">
                  <input
                    type="checkbox"
                    v-model="perm.can_view"
                    class="rounded border-gray-300 text-primary-600 focus:ring-primary-500"
                  />
                </td>
                <td class="whitespace-nowrap px-3 py-4 text-sm text-gray-500 text-center">
                  <input
                    type="checkbox"
                    v-model="perm.can_create"
                    class="rounded border-gray-300 text-primary-600 focus:ring-primary-500"
                  />
                </td>
                <td class="whitespace-nowrap px-3 py-4 text-sm text-gray-500 text-center">
                  <input
                    type="checkbox"
                    v-model="perm.can_edit"
                    class="rounded border-gray-300 text-primary-600 focus:ring-primary-500"
                  />
                </td>
                <td class="whitespace-nowrap px-3 py-4 text-sm text-gray-500 text-center">
                  <input
                    type="checkbox"
                    v-model="perm.can_delete"
                    class="rounded border-gray-300 text-primary-600 focus:ring-primary-500"
                  />
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <template #footer>
        <div class="flex justify-end space-x-3">
          <BaseButton variant="outline" @click="showPermissionsModal = false" :disabled="adminStore.loading">
            Cancelar
          </BaseButton>
          <BaseButton variant="primary" @click="savePermissions" :loading="adminStore.loading">
            Guardar Permisos
          </BaseButton>
        </div>
      </template>
    </BaseModal>
  </div>
</template>

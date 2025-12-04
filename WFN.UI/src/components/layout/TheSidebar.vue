<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink, useRoute } from 'vue-router'
import {
  HomeIcon,
  UsersIcon,
  BuildingOfficeIcon,
  CurrencyDollarIcon,
  Cog6ToothIcon,
  FolderIcon,
  DocumentChartBarIcon,
  ShieldCheckIcon,
} from '@heroicons/vue/24/outline'
import { useAuthStore } from '@/stores'
import type { ModuleName } from '@/types'

const route = useRoute()
const authStore = useAuthStore()

interface MenuItem {
  name: string
  route: string
  icon: ReturnType<typeof HomeIcon>
  moduleName: ModuleName
}

const menuItems: MenuItem[] = [
  { name: 'Dashboard', route: '/dashboard', icon: HomeIcon, moduleName: 'dashboard' },
  { name: 'Personas', route: '/personas', icon: UsersIcon, moduleName: 'personas' },
  { name: 'Empleados', route: '/empleados', icon: UsersIcon, moduleName: 'empleados' },
  {
    name: 'Departamentos',
    route: '/departamentos',
    icon: BuildingOfficeIcon,
    moduleName: 'departamentos',
  },
  { name: 'Nóminas', route: '/nominas', icon: CurrencyDollarIcon, moduleName: 'nominas' },
  { name: 'Parámetros', route: '/parametros', icon: Cog6ToothIcon, moduleName: 'parametros' },
  { name: 'Reportes', route: '/reportes', icon: DocumentChartBarIcon, moduleName: 'reportes' },
]

const adminMenuItem: MenuItem = {
  name: 'Administración',
  route: '/administracion',
  icon: ShieldCheckIcon,
  moduleName: 'administracion',
}

const filteredMenuItems = computed(() => {
  return menuItems.filter((item) => authStore.canAccessModule(item.moduleName))
})

// SUPER_ADMIN always has access to admin menu
// Also check if user has explicit permission for administracion module
const showAdminMenu = computed(() => {
  const isSuperAdmin = authStore.isSuperAdmin
  const hasAdminPermission = authStore.canAccessModule('administracion')

  // Debug logging
  if (import.meta.env.DEV) {
    console.log('🔐 Admin Menu Check:', {
      isSuperAdmin,
      hasAdminPermission,
      userRole: authStore.userRole?.name,
      showMenu: isSuperAdmin || hasAdminPermission
    })
  }

  return isSuperAdmin || hasAdminPermission
})

const isActive = (itemRoute: string): boolean => {
  return route.path.startsWith(itemRoute)
}
</script>

<template>
  <aside class="fixed inset-y-0 left-0 z-50 w-64 bg-white border-r border-gray-200 flex flex-col">
    <!-- Logo -->
    <div class="flex items-center h-16 px-6 border-b border-gray-200">
      <FolderIcon class="h-8 w-8 text-primary-600" />
      <span class="ml-2 text-xl font-bold text-gray-900">WFN System</span>
    </div>

    <!-- Navigation -->
    <nav class="flex-1 px-4 py-4 space-y-1 overflow-y-auto">
      <!-- Warning: No permissions -->
      <div v-if="filteredMenuItems.length === 0 && !authStore.isSuperAdmin" class="mb-4 p-3 bg-yellow-50 border border-yellow-200 rounded-lg">
        <div class="flex">
          <svg class="h-5 w-5 text-yellow-400" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clip-rule="evenodd" />
          </svg>
          <div class="ml-3">
            <h3 class="text-xs font-medium text-yellow-800">Sin permisos</h3>
            <p class="mt-1 text-xs text-yellow-700">
              No tienes acceso a ningún módulo. Contacta al administrador.
            </p>
          </div>
        </div>
      </div>

      <RouterLink
        v-for="item in filteredMenuItems"
        :key="item.route"
        :to="item.route"
        :class="[
          'sidebar-link',
          isActive(item.route) ? 'sidebar-link-active' : 'sidebar-link-inactive',
        ]"
      >
        <component :is="item.icon" class="h-5 w-5 mr-3" />
        {{ item.name }}
      </RouterLink>

      <!-- Admin Section -->
      <template v-if="showAdminMenu">
        <div class="pt-4 mt-4 border-t border-gray-200">
          <p class="px-4 text-xs font-semibold text-gray-400 uppercase tracking-wider">
            Administración
          </p>
          <RouterLink
            :to="adminMenuItem.route"
            :class="[
              'sidebar-link mt-2',
              isActive(adminMenuItem.route) ? 'sidebar-link-active' : 'sidebar-link-inactive',
            ]"
          >
            <component :is="adminMenuItem.icon" class="h-5 w-5 mr-3" />
            {{ adminMenuItem.name }}
          </RouterLink>
        </div>
      </template>
    </nav>

    <!-- User Info -->
    <div class="p-4 border-t border-gray-200">
      <div class="flex items-center">
        <div
          class="h-8 w-8 rounded-full bg-primary-100 flex items-center justify-center text-primary-600 font-medium"
        >
          {{ authStore.userProfile?.first_name?.charAt(0) || 'U' }}
        </div>
        <div class="ml-3 flex-1 min-w-0">
          <p class="text-sm font-medium text-gray-900 truncate">
            {{ authStore.fullName || 'Usuario' }}
          </p>
          <p class="text-xs text-gray-500 truncate">
            {{ authStore.userRole?.name || 'Sin rol' }}
          </p>
        </div>
      </div>
    </div>
  </aside>
</template>

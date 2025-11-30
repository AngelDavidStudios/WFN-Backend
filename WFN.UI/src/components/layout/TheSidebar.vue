<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink, useRoute } from 'vue-router'
import {
  HomeIcon,
  UsersIcon,
  BuildingOfficeIcon,
  CurrencyDollarIcon,
  DocumentTextIcon,
  BanknotesIcon,
  ChartBarIcon,
  Cog6ToothIcon,
  FolderIcon,
  CalendarIcon,
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
  { name: 'Novedades', route: '/novedades', icon: DocumentTextIcon, moduleName: 'novedades' },
  { name: 'Banking', route: '/banking', icon: BanknotesIcon, moduleName: 'banking' },
  { name: 'Provisiones', route: '/provisiones', icon: ChartBarIcon, moduleName: 'provisiones' },
  { name: 'Parámetros', route: '/parametros', icon: Cog6ToothIcon, moduleName: 'parametros' },
  { name: 'Workspaces', route: '/workspaces', icon: CalendarIcon, moduleName: 'workspaces' },
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

const showAdminMenu = computed(() => authStore.isSuperAdmin)

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

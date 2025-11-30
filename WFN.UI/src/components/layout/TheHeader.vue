<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import {
  Bars3Icon,
  BellIcon,
  ArrowRightOnRectangleIcon,
  UserCircleIcon,
} from '@heroicons/vue/24/outline'
import { Menu, MenuButton, MenuItem, MenuItems } from '@headlessui/vue'
import { useAuthStore, useUIStore } from '@/stores'

const router = useRouter()
const authStore = useAuthStore()
const uiStore = useUIStore()

const showNotifications = ref(false)

async function handleLogout() {
  await authStore.logout()
  router.push('/login')
}

function toggleSidebar() {
  uiStore.toggleSidebar()
}
</script>

<template>
  <header class="sticky top-0 z-40 bg-white border-b border-gray-200">
    <div class="flex items-center justify-between h-16 px-6">
      <!-- Left side -->
      <div class="flex items-center">
        <button
          type="button"
          class="p-2 text-gray-500 hover:text-gray-600 lg:hidden"
          @click="toggleSidebar"
        >
          <Bars3Icon class="h-6 w-6" />
        </button>
      </div>

      <!-- Right side -->
      <div class="flex items-center space-x-4">
        <!-- Notifications -->
        <button
          type="button"
          class="p-2 text-gray-500 hover:text-gray-600 relative"
          @click="showNotifications = !showNotifications"
        >
          <BellIcon class="h-6 w-6" />
          <span
            v-if="uiStore.notifications.length > 0"
            class="absolute top-1 right-1 h-2 w-2 bg-red-500 rounded-full"
          ></span>
        </button>

        <!-- User Menu -->
        <Menu as="div" class="relative">
          <MenuButton
            class="flex items-center space-x-3 p-2 rounded-md hover:bg-gray-100 transition-colors"
          >
            <div
              class="h-8 w-8 rounded-full bg-primary-100 flex items-center justify-center text-primary-600 font-medium"
            >
              {{ authStore.userProfile?.first_name?.charAt(0) || 'U' }}
            </div>
            <span class="hidden md:block text-sm font-medium text-gray-700">
              {{ authStore.fullName || 'Usuario' }}
            </span>
          </MenuButton>

          <transition
            enter-active-class="transition duration-100 ease-out"
            enter-from-class="transform scale-95 opacity-0"
            enter-to-class="transform scale-100 opacity-100"
            leave-active-class="transition duration-75 ease-in"
            leave-from-class="transform scale-100 opacity-100"
            leave-to-class="transform scale-95 opacity-0"
          >
            <MenuItems
              class="absolute right-0 mt-2 w-48 origin-top-right bg-white rounded-md shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none"
            >
              <div class="py-1">
                <MenuItem v-slot="{ active }">
                  <a
                    href="#"
                    :class="[
                      active ? 'bg-gray-100' : '',
                      'flex items-center px-4 py-2 text-sm text-gray-700',
                    ]"
                  >
                    <UserCircleIcon class="h-5 w-5 mr-3 text-gray-400" />
                    Mi Perfil
                  </a>
                </MenuItem>
                <MenuItem v-slot="{ active }">
                  <button
                    type="button"
                    :class="[
                      active ? 'bg-gray-100' : '',
                      'flex items-center w-full px-4 py-2 text-sm text-gray-700',
                    ]"
                    @click="handleLogout"
                  >
                    <ArrowRightOnRectangleIcon class="h-5 w-5 mr-3 text-gray-400" />
                    Cerrar Sesión
                  </button>
                </MenuItem>
              </div>
            </MenuItems>
          </transition>
        </Menu>
      </div>
    </div>
  </header>
</template>

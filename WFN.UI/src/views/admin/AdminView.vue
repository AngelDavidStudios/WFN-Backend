<script setup lang="ts">
import { ref } from 'vue'
import UserList from './components/UserList.vue'
import RoleList from './components/RoleList.vue'

const activeTab = ref('users')
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <h1 class="text-2xl font-semibold text-gray-900">Administración</h1>
    </div>

    <!-- Tabs -->
    <div class="border-b border-gray-200">
      <nav class="-mb-px flex space-x-8" aria-label="Tabs">
        <button
          @click="activeTab = 'users'"
          :class="[
            activeTab === 'users'
              ? 'border-primary-500 text-primary-600'
              : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300',
            'whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm',
          ]"
        >
          Usuarios
        </button>
        <button
          @click="activeTab = 'roles'"
          :class="[
            activeTab === 'roles'
              ? 'border-primary-500 text-primary-600'
              : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300',
            'whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm',
          ]"
        >
          Roles y Permisos
        </button>
      </nav>
    </div>

    <!-- Content -->
    <div class="mt-6">
      <UserList v-if="activeTab === 'users'" />
      <RoleList v-if="activeTab === 'roles'" />
    </div>
  </div>
</template>

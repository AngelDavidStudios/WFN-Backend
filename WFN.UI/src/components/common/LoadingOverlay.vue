<script setup lang="ts">
import { useUIStore } from '@/stores'

const uiStore = useUIStore()
</script>

<template>
  <Transition
    enter-active-class="transition-opacity duration-300"
    enter-from-class="opacity-0"
    enter-to-class="opacity-100"
    leave-active-class="transition-opacity duration-200"
    leave-from-class="opacity-100"
    leave-to-class="opacity-0"
  >
    <div
      v-if="uiStore.isLoading"
      class="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50"
    >
      <div class="bg-white rounded-lg p-6 shadow-xl flex items-center space-x-4">
        <div
          class="animate-spin h-8 w-8 border-4 border-primary-600 border-t-transparent rounded-full"
        ></div>
        <span class="text-gray-700 font-medium">
          {{ uiStore.loadingMessage || 'Cargando...' }}
        </span>
      </div>
    </div>
  </Transition>
</template>

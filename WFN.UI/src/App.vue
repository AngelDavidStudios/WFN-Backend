<script setup lang="ts">
import { RouterView } from 'vue-router'
import { onErrorCaptured, ref } from 'vue'

const error = ref<Error | null>(null)

onErrorCaptured((err) => {
  console.error('App error captured:', err)
  error.value = err
  return false
})
</script>

<template>
  <div v-if="error" class="min-h-screen flex items-center justify-center bg-gray-100">
    <div class="max-w-md w-full bg-white shadow-lg rounded-lg p-6">
      <h1 class="text-2xl font-bold text-red-600 mb-4">Error</h1>
      <p class="text-gray-700 mb-4">{{ error.message }}</p>
      <button
        class="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
        @click="() => window.location.reload()"
      >
        Recargar Página
      </button>
    </div>
  </div>
  <Suspense v-else>
    <RouterView />
    <template #fallback>
      <div class="min-h-screen flex items-center justify-center">
        <div class="animate-spin h-12 w-12 border-4 border-blue-600 border-t-transparent rounded-full"></div>
      </div>
    </template>
  </Suspense>
</template>

<style scoped></style>

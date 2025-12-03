<script setup lang="ts">
import { RouterView, useRouter } from 'vue-router'
import { onErrorCaptured, ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores'

const router = useRouter()
const authStore = useAuthStore()
const error = ref<Error | null>(null)
const isReady = ref(false)

onMounted(async () => {
  // Safety timeout in case router.isReady() hangs
  const timeout = setTimeout(() => {
    console.warn('Router initialization timed out, forcing ready state')
    isReady.value = true
  }, 4000) // Reducido a 4 segundos

  try {
    await router.isReady()
  } catch (e) {
    console.error('Router isReady error:', e)
  } finally {
    clearTimeout(timeout)
    isReady.value = true
  }
})

onErrorCaptured((err) => {
  console.error('App error captured:', err)
  error.value = err
  return false
})

const reloadPage = () => {
  window.location.reload()
}

const retryConnection = async () => {
  await authStore.checkSession()
}
</script>

<template>
  <div v-if="error" class="min-h-screen flex items-center justify-center bg-gray-100">
    <div class="max-w-md w-full bg-white shadow-lg rounded-lg p-6">
      <h1 class="text-2xl font-bold text-red-600 mb-4">Error</h1>
      <p class="text-gray-700 mb-4">{{ error.message }}</p>
      <button
        class="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
        @click="reloadPage"
      >
        Recargar Página
      </button>
    </div>
  </div>

  <div
    v-else-if="authStore.connectionError"
    class="min-h-screen flex items-center justify-center bg-gray-100"
  >
    <div class="max-w-md w-full bg-white shadow-lg rounded-lg p-6 text-center">
      <div class="text-red-500 text-5xl mb-4">⚠️</div>
      <h1 class="text-2xl font-bold text-gray-800 mb-2">Error de Conexión</h1>
      <p class="text-gray-600 mb-6">{{ authStore.connectionError }}</p>
      <button
        class="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition-colors"
        @click="retryConnection"
      >
        Reintentar Conexión
      </button>
    </div>
  </div>

  <div v-else-if="!isReady" class="min-h-screen flex items-center justify-center">
    <div
      class="animate-spin h-12 w-12 border-4 border-blue-600 border-t-transparent rounded-full"
    ></div>
  </div>

  <Suspense v-else>
    <RouterView />
    <template #fallback>
      <div class="min-h-screen flex items-center justify-center">
        <div
          class="animate-spin h-12 w-12 border-4 border-blue-600 border-t-transparent rounded-full"
        ></div>
      </div>
    </template>
  </Suspense>
</template>

<style scoped></style>

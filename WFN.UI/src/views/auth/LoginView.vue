<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore, useUIStore } from '@/stores'
import BaseButton from '@/components/common/BaseButton.vue'
import FormInput from '@/components/forms/FormInput.vue'
import { FolderIcon } from '@heroicons/vue/24/outline'

const router = useRouter()
const authStore = useAuthStore()
const uiStore = useUIStore()

const form = ref({
  email: '',
  password: '',
})
const loading = ref(false)
const error = ref<string | null>(null)

async function handleLogin() {
  if (!form.value.email || !form.value.password) {
    error.value = 'Por favor complete todos los campos'
    return
  }

  loading.value = true
  error.value = null

  try {
    await authStore.login({
      email: form.value.email,
      password: form.value.password,
    })
    uiStore.notifySuccess('Bienvenido', 'Sesión iniciada correctamente')
    router.push('/dashboard')
  } catch (err) {
    if (err instanceof Error) {
      if (err.message.includes('Invalid login credentials')) {
        error.value = 'Email o contraseña incorrectos'
      } else {
        error.value = err.message
      }
    } else {
      error.value = 'Error al iniciar sesión'
    }
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
      <!-- Logo and Title -->
      <div class="text-center">
        <div class="flex justify-center">
          <div class="h-16 w-16 bg-primary-100 rounded-full flex items-center justify-center">
            <FolderIcon class="h-10 w-10 text-primary-600" />
          </div>
        </div>
        <h2 class="mt-6 text-3xl font-extrabold text-gray-900">Sistema de Nóminas WFN</h2>
        <p class="mt-2 text-sm text-gray-600">Ingrese sus credenciales para acceder</p>
      </div>

      <!-- Login Form -->
      <form class="mt-8 space-y-6" @submit.prevent="handleLogin">
        <div class="card">
          <div class="card-body space-y-4">
            <!-- Error Message -->
            <div
              v-if="error"
              class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-md text-sm"
            >
              {{ error }}
            </div>

            <!-- Email -->
            <FormInput
              v-model="form.email"
              type="email"
              label="Correo Electrónico"
              placeholder="correo@empresa.com"
              required
            />

            <!-- Password -->
            <FormInput
              v-model="form.password"
              type="password"
              label="Contraseña"
              placeholder="••••••••"
              required
            />

            <!-- Submit Button -->
            <BaseButton type="submit" variant="primary" :loading="loading" block>
              {{ loading ? 'Ingresando...' : 'Iniciar Sesión' }}
            </BaseButton>
          </div>
        </div>
      </form>

      <!-- Footer -->
      <p class="text-center text-sm text-gray-500">
        © {{ new Date().getFullYear() }} WFN System. Todos los derechos reservados.
      </p>
    </div>
  </div>
</template>

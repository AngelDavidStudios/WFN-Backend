<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { supabase } from '@/utils/supabase'
import { CheckCircleIcon, XCircleIcon } from '@heroicons/vue/24/outline'

const router = useRouter()
const status = ref<'loading' | 'success' | 'error'>('loading')
const errorMessage = ref('')

onMounted(async () => {
  console.log('📧 Email confirmation page loaded')

  try {
    // Obtener el hash de la URL (contiene el token de confirmación)
    const hashParams = new URLSearchParams(window.location.hash.substring(1))
    const accessToken = hashParams.get('access_token')
    const type = hashParams.get('type')

    console.log('🔍 URL params:', { type, hasToken: !!accessToken })

    // Verificar que sea una confirmación de email
    if (type === 'signup' || type === 'email' || type === 'magiclink') {
      // El token ya fue procesado automáticamente por Supabase
      // Solo necesitamos verificar la sesión
      const { data: { session }, error: sessionError } = await supabase.auth.getSession()

      if (sessionError) {
        console.error('❌ Session error:', sessionError)
        throw sessionError
      }

      if (session) {
        console.log('✅ Email confirmed successfully!')
        status.value = 'success'

        // Redirigir al dashboard después de 3 segundos
        setTimeout(() => {
          router.push('/dashboard')
        }, 3000)
      } else {
        throw new Error('No se pudo verificar la sesión')
      }
    } else {
      // Si no hay type, verificar si hay un error en la URL
      const errorParam = hashParams.get('error')
      const errorDescription = hashParams.get('error_description')

      if (errorParam) {
        throw new Error(errorDescription || errorParam)
      } else {
        throw new Error('Link de confirmación inválido o expirado')
      }
    }
  } catch (err: any) {
    console.error('❌ Email confirmation error:', err)
    status.value = 'error'
    errorMessage.value = err.message || 'Error al confirmar el email'
  }
})

const goToLogin = () => {
  router.push('/login')
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-primary-50 to-primary-100 px-4">
    <div class="max-w-md w-full">
      <!-- Loading State -->
      <div v-if="status === 'loading'" class="bg-white rounded-2xl shadow-xl p-8 text-center">
        <div class="inline-flex items-center justify-center w-16 h-16 bg-primary-100 rounded-full mb-4 animate-pulse">
          <svg class="animate-spin h-8 w-8 text-primary-600" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
        </div>
        <h2 class="text-2xl font-bold text-gray-900 mb-2">
          Confirmando tu email...
        </h2>
        <p class="text-gray-600">
          Por favor espera un momento
        </p>
      </div>

      <!-- Success State -->
      <div v-else-if="status === 'success'" class="bg-white rounded-2xl shadow-xl p-8 text-center">
        <div class="inline-flex items-center justify-center w-16 h-16 bg-green-100 rounded-full mb-4">
          <CheckCircleIcon class="h-10 w-10 text-green-600" />
        </div>
        <h2 class="text-2xl font-bold text-gray-900 mb-2">
          ¡Email Confirmado!
        </h2>
        <p class="text-gray-600 mb-6">
          Tu cuenta ha sido activada exitosamente.
        </p>
        <div class="bg-green-50 border border-green-200 rounded-lg p-4 mb-6">
          <p class="text-sm text-green-800">
            Serás redirigido al dashboard en unos segundos...
          </p>
        </div>
        <div class="flex items-center justify-center space-x-2 text-sm text-gray-500">
          <div class="animate-bounce">•</div>
          <div class="animate-bounce animation-delay-200">•</div>
          <div class="animate-bounce animation-delay-400">•</div>
        </div>
      </div>

      <!-- Error State -->
      <div v-else-if="status === 'error'" class="bg-white rounded-2xl shadow-xl p-8 text-center">
        <div class="inline-flex items-center justify-center w-16 h-16 bg-red-100 rounded-full mb-4">
          <XCircleIcon class="h-10 w-10 text-red-600" />
        </div>
        <h2 class="text-2xl font-bold text-gray-900 mb-2">
          Error al Confirmar
        </h2>
        <p class="text-gray-600 mb-4">
          {{ errorMessage }}
        </p>
        <div class="bg-red-50 border border-red-200 rounded-lg p-4 mb-6">
          <p class="text-sm text-red-800 mb-2">
            <strong>Posibles causas:</strong>
          </p>
          <ul class="text-xs text-red-700 text-left list-disc list-inside space-y-1">
            <li>El link de confirmación ha expirado</li>
            <li>El link ya fue usado anteriormente</li>
            <li>El link es inválido o está corrupto</li>
          </ul>
        </div>
        <button
          @click="goToLogin"
          class="w-full bg-primary-600 text-white py-3 px-4 rounded-lg hover:bg-primary-700 transition-colors font-medium"
        >
          Ir al Inicio de Sesión
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.animation-delay-200 {
  animation-delay: 0.2s;
}

.animation-delay-400 {
  animation-delay: 0.4s;
}
</style>


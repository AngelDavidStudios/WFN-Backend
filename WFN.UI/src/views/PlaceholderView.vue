<script setup lang="ts">
import { useRoute } from 'vue-router'
import { computed } from 'vue'
import { WrenchScrewdriverIcon } from '@heroicons/vue/24/outline'

const route = useRoute()

const title = computed(() => (route.meta.title as string) || 'Módulo')
</script>

<template>
  <div class="flex flex-col items-center justify-center py-16">
    <div class="bg-gray-100 rounded-full p-6 mb-6">
      <WrenchScrewdriverIcon class="h-16 w-16 text-gray-400" />
    </div>
    <h1 class="text-2xl font-bold text-gray-900 mb-2">{{ title }}</h1>
    <p class="text-gray-500 text-center max-w-md">
      Este módulo está en desarrollo. Pronto estará disponible con todas sus funcionalidades.
    </p>
  </div>
</template>

<script setup lang="ts">
import {
  CheckCircleIcon,
  XCircleIcon,
  ExclamationTriangleIcon,
  InformationCircleIcon,
  XMarkIcon,
} from '@heroicons/vue/24/outline'
import { useUIStore } from '@/stores'

const uiStore = useUIStore()

const iconMap = {
  success: CheckCircleIcon,
  error: XCircleIcon,
  warning: ExclamationTriangleIcon,
  info: InformationCircleIcon,
}

const colorMap = {
  success: 'bg-green-50 text-green-800 border-green-200',
  error: 'bg-red-50 text-red-800 border-red-200',
  warning: 'bg-yellow-50 text-yellow-800 border-yellow-200',
  info: 'bg-blue-50 text-blue-800 border-blue-200',
}

const iconColorMap = {
  success: 'text-green-500',
  error: 'text-red-500',
  warning: 'text-yellow-500',
  info: 'text-blue-500',
}
</script>

<template>
  <div class="fixed bottom-4 right-4 z-50 space-y-2 max-w-sm w-full pointer-events-none">
    <TransitionGroup
      enter-active-class="transform transition duration-300 ease-out"
      enter-from-class="translate-x-full opacity-0"
      enter-to-class="translate-x-0 opacity-100"
      leave-active-class="transform transition duration-200 ease-in"
      leave-from-class="translate-x-0 opacity-100"
      leave-to-class="translate-x-full opacity-0"
    >
      <div
        v-for="notification in uiStore.notifications"
        :key="notification.id"
        :class="[
          'pointer-events-auto rounded-lg border p-4 shadow-lg',
          colorMap[notification.type],
        ]"
      >
        <div class="flex items-start">
          <component
            :is="iconMap[notification.type]"
            :class="['h-5 w-5 flex-shrink-0', iconColorMap[notification.type]]"
          />
          <div class="ml-3 flex-1">
            <p class="text-sm font-medium">{{ notification.title }}</p>
            <p v-if="notification.message" class="mt-1 text-sm opacity-80">
              {{ notification.message }}
            </p>
          </div>
          <button
            type="button"
            class="ml-4 inline-flex flex-shrink-0 rounded-md hover:opacity-70 focus:outline-none"
            @click="uiStore.removeNotification(notification.id)"
          >
            <XMarkIcon class="h-5 w-5" />
          </button>
        </div>
      </div>
    </TransitionGroup>
  </div>
</template>

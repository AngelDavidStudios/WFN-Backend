import { ref } from 'vue'
import { defineStore } from 'pinia'

export interface Notification {
  id: string
  type: 'success' | 'error' | 'warning' | 'info'
  title: string
  message?: string
  duration?: number
}

export const useUIStore = defineStore('ui', () => {
  // State
  const isLoading = ref(false)
  const loadingMessage = ref('')
  const notifications = ref<Notification[]>([])
  const sidebarCollapsed = ref(false)

  // Actions
  function setLoading(loading: boolean, message = ''): void {
    isLoading.value = loading
    loadingMessage.value = message
  }

  function addNotification(notification: Omit<Notification, 'id'>): void {
    const id = Date.now().toString()
    const newNotification: Notification = {
      id,
      ...notification,
      duration: notification.duration ?? 5000,
    }
    notifications.value.push(newNotification)

    // Auto-remove after duration
    const duration = newNotification.duration ?? 5000
    if (duration > 0) {
      setTimeout(() => {
        removeNotification(id)
      }, duration)
    }
  }

  function removeNotification(id: string): void {
    const index = notifications.value.findIndex((n) => n.id === id)
    if (index !== -1) {
      notifications.value.splice(index, 1)
    }
  }

  function clearNotifications(): void {
    notifications.value = []
  }

  // Convenience methods for different notification types
  function notifySuccess(title: string, message?: string): void {
    addNotification({ type: 'success', title, message })
  }

  function notifyError(title: string, message?: string): void {
    addNotification({ type: 'error', title, message, duration: 10000 })
  }

  function notifyWarning(title: string, message?: string): void {
    addNotification({ type: 'warning', title, message })
  }

  function notifyInfo(title: string, message?: string): void {
    addNotification({ type: 'info', title, message })
  }

  function toggleSidebar(): void {
    sidebarCollapsed.value = !sidebarCollapsed.value
  }

  return {
    // State
    isLoading,
    loadingMessage,
    notifications,
    sidebarCollapsed,
    // Actions
    setLoading,
    addNotification,
    removeNotification,
    clearNotifications,
    notifySuccess,
    notifyError,
    notifyWarning,
    notifyInfo,
    toggleSidebar,
  }
})

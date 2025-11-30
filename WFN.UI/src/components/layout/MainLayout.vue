<script setup lang="ts">
import { RouterView } from 'vue-router'
import TheSidebar from './TheSidebar.vue'
import TheHeader from './TheHeader.vue'
import NotificationContainer from '../common/NotificationContainer.vue'
import LoadingOverlay from '../common/LoadingOverlay.vue'
</script>

<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Sidebar -->
    <TheSidebar />

    <!-- Main Content -->
    <div class="lg:pl-64">
      <!-- Header -->
      <TheHeader />

      <!-- Page Content -->
      <main class="p-6">
        <RouterView />
      </main>
    </div>

    <!-- Notifications -->
    <NotificationContainer />

    <!-- Loading Overlay -->
    <LoadingOverlay />
  </div>
</template>

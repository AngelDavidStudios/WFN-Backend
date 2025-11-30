<script setup lang="ts">
interface Props {
  size?: 'sm' | 'md' | 'lg'
  color?: string
}

const props = withDefaults(defineProps<Props>(), {
  size: 'md',
  color: 'text-primary-600',
})

const sizeClasses = {
  sm: 'h-4 w-4 border-2',
  md: 'h-8 w-8 border-4',
  lg: 'h-12 w-12 border-4',
}
</script>

<template>
  <div
    :class="[
      'animate-spin rounded-full border-t-transparent',
      sizeClasses[props.size],
      props.color.includes('border') ? props.color : `border-current ${props.color}`,
    ]"
  ></div>
</template>

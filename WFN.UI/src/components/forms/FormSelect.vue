<script setup lang="ts">
import { computed } from 'vue'
import type { DropdownOption } from '@/types'

interface Props {
  modelValue: string | number | boolean
  label?: string
  options: DropdownOption<string | number | boolean>[]
  placeholder?: string
  error?: string
  required?: boolean
  disabled?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: 'Seleccione una opción',
  error: '',
  required: false,
  disabled: false,
})

const emit = defineEmits<{
  'update:modelValue': [value: string | number | boolean]
}>()

const selectValue = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value),
})

const selectClasses = computed(() => [
  'input',
  props.error ? 'input-error' : '',
])
</script>

<template>
  <div class="w-full">
    <label v-if="props.label" class="label">
      {{ props.label }}
      <span v-if="props.required" class="text-red-500">*</span>
    </label>
    <select
      v-model="selectValue"
      :required="props.required"
      :disabled="props.disabled"
      :class="selectClasses"
    >
      <option value="" disabled>{{ props.placeholder }}</option>
      <option
        v-for="option in props.options"
        :key="String(option.value)"
        :value="option.value"
        :disabled="option.disabled"
      >
        {{ option.label }}
      </option>
    </select>
    <p v-if="props.error" class="mt-1 text-sm text-red-600">
      {{ props.error }}
    </p>
  </div>
</template>

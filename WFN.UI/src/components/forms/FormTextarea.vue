<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  modelValue: string
  label?: string
  placeholder?: string
  rows?: number
  error?: string
  required?: boolean
  disabled?: boolean
  readonly?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: '',
  rows: 3,
  error: '',
  required: false,
  disabled: false,
  readonly: false,
})

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const textareaValue = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value),
})

const textareaClasses = computed(() => [
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
    <textarea
      v-model="textareaValue"
      :placeholder="props.placeholder"
      :rows="props.rows"
      :required="props.required"
      :disabled="props.disabled"
      :readonly="props.readonly"
      :class="textareaClasses"
    ></textarea>
    <p v-if="props.error" class="mt-1 text-sm text-red-600">
      {{ props.error }}
    </p>
  </div>
</template>

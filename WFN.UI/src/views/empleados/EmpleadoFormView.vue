<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { api } from '@/api'
import { useUIStore } from '@/stores'
import type { EmpleadoCreateDTO, Persona, Departamento, DropdownOption } from '@/types'
import { StatusLaboral } from '@/types'
import { STATUS_LABORAL_OPTIONS } from '@/constants'
import { hasWorkedMoreThanOneYear } from '@/utils'
import FormInput from '@/components/forms/FormInput.vue'
import FormSelect from '@/components/forms/FormSelect.vue'
import FormCheckbox from '@/components/forms/FormCheckbox.vue'
import BaseButton from '@/components/common/BaseButton.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

const router = useRouter()
const route = useRoute()
const uiStore = useUIStore()

const isEdit = computed(() => !!route.params.id)
const pageTitle = computed(() => (isEdit.value ? 'Editar Empleado' : 'Nuevo Empleado'))

const loading = ref(false)
const saving = ref(false)
const errors = ref<Record<string, string>>({})

// Dropdown options
const personasOptions = ref<DropdownOption<string>[]>([])
const departamentosOptions = ref<DropdownOption<string>[]>([])

const form = ref<EmpleadoCreateDTO>({
  id_Persona: '',
  id_Departamento: '',
  fechaIngreso: '',
  salarioBase: 0,
  is_DecimoTercMensual: false,
  is_DecimoCuartoMensual: false,
  is_FondoReserva: false,
  statusLaboral: StatusLaboral.Active,
})

async function loadDropdownData() {
  try {
    const [personas, departamentos] = await Promise.all([
      api.persona.getAll(),
      api.departamento.getAll(),
    ])

    personasOptions.value = personas.map((p: Persona) => ({
      value: p.id_Persona,
      label: `${p.primerNombre} ${p.apellidoPaterno} (${p.dni})`,
    }))

    departamentosOptions.value = departamentos.map((d: Departamento) => ({
      value: d.id_Departamento,
      label: d.nombre,
    }))
  } catch (error) {
    console.error('Error loading dropdown data:', error)
    uiStore.notifyError('Error', 'No se pudieron cargar los datos')
  }
}

async function loadEmpleado() {
  if (!isEdit.value) return

  loading.value = true
  try {
    const empleado = await api.empleado.getById(route.params.id as string)
    form.value = {
      id_Persona: empleado.id_Persona,
      id_Departamento: empleado.id_Departamento,
      fechaIngreso: empleado.fechaIngreso?.split('T')[0] || '',
      salarioBase: empleado.salarioBase,
      is_DecimoTercMensual: empleado.is_DecimoTercMensual,
      is_DecimoCuartoMensual: empleado.is_DecimoCuartoMensual,
      is_FondoReserva: empleado.is_FondoReserva,
      statusLaboral: empleado.statusLaboral,
    }
  } catch (error) {
    console.error('Error loading empleado:', error)
    uiStore.notifyError('Error', 'No se pudo cargar el empleado')
    router.push('/empleados')
  } finally {
    loading.value = false
  }
}

function validate(): boolean {
  errors.value = {}

  if (!form.value.id_Persona) errors.value.id_Persona = 'Debe seleccionar una persona'
  if (!form.value.id_Departamento) errors.value.id_Departamento = 'Debe seleccionar un departamento'
  if (!form.value.fechaIngreso) errors.value.fechaIngreso = 'La fecha de ingreso es requerida'
  if (form.value.salarioBase <= 0) errors.value.salarioBase = 'El salario debe ser mayor a 0'

  // Validar fondos de reserva (solo si tiene más de 1 año)
  if (form.value.is_FondoReserva && form.value.fechaIngreso) {
    if (!hasWorkedMoreThanOneYear(form.value.fechaIngreso)) {
      errors.value.is_FondoReserva = 'Fondos de reserva solo aplica para empleados con más de 1 año'
    }
  }

  return Object.keys(errors.value).length === 0
}

async function handleSubmit() {
  if (!validate()) return

  saving.value = true
  try {
    if (isEdit.value) {
      await api.empleado.update(route.params.id as string, {
        ...form.value,
        id_Empleado: route.params.id as string,
      })
      uiStore.notifySuccess('Éxito', 'Empleado actualizado correctamente')
    } else {
      await api.empleado.create(form.value)
      uiStore.notifySuccess('Éxito', 'Empleado creado correctamente')
    }
    router.push('/empleados')
  } catch (error) {
    console.error('Error saving empleado:', error)
    uiStore.notifyError('Error', 'No se pudo guardar el empleado')
  } finally {
    saving.value = false
  }
}

function goBack() {
  router.push('/empleados')
}

onMounted(async () => {
  await loadDropdownData()
  await loadEmpleado()
})
</script>

<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">{{ pageTitle }}</h1>
        <p class="mt-1 text-sm text-gray-500">
          {{ isEdit ? 'Modifique los datos del empleado' : 'Complete los datos del nuevo empleado' }}
        </p>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="flex justify-center py-12">
      <LoadingSpinner size="lg" />
    </div>

    <!-- Form -->
    <form v-else class="space-y-6" @submit.prevent="handleSubmit">
      <!-- Basic Information -->
      <div class="card">
        <div class="card-header">
          <h2 class="text-lg font-medium text-gray-900">Información del Empleado</h2>
        </div>
        <div class="card-body">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <FormSelect
              v-model="form.id_Persona"
              label="Persona"
              :options="personasOptions"
              placeholder="Seleccione una persona"
              required
              :error="errors.id_Persona"
              :disabled="isEdit"
            />
            <FormSelect
              v-model="form.id_Departamento"
              label="Departamento"
              :options="departamentosOptions"
              placeholder="Seleccione un departamento"
              required
              :error="errors.id_Departamento"
            />
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mt-4">
            <FormInput
              v-model="form.fechaIngreso"
              type="date"
              label="Fecha de Ingreso"
              required
              :error="errors.fechaIngreso"
            />
            <FormInput
              v-model="form.salarioBase"
              type="number"
              label="Salario Base ($)"
              placeholder="0.00"
              required
              :error="errors.salarioBase"
            />
          </div>

          <div class="mt-4">
            <FormSelect
              v-model="form.statusLaboral"
              label="Estado Laboral"
              :options="STATUS_LABORAL_OPTIONS"
              required
            />
          </div>
        </div>
      </div>

      <!-- Payroll Configuration -->
      <div class="card">
        <div class="card-header">
          <h2 class="text-lg font-medium text-gray-900">Configuración de Nómina</h2>
          <p class="mt-1 text-sm text-gray-500">Configure cómo se calculan los beneficios</p>
        </div>
        <div class="card-body">
          <div class="space-y-4">
            <FormCheckbox
              v-model="form.is_DecimoTercMensual"
              label="Décimo Tercero Mensualizado"
            />
            <p class="text-sm text-gray-500 ml-6">
              Si está activo, el décimo tercero se paga mensualmente (1/12 del salario)
            </p>

            <FormCheckbox
              v-model="form.is_DecimoCuartoMensual"
              label="Décimo Cuarto Mensualizado"
            />
            <p class="text-sm text-gray-500 ml-6">
              Si está activo, el décimo cuarto se paga mensualmente ($470/12 = $39.17)
            </p>

            <FormCheckbox
              v-model="form.is_FondoReserva"
              label="Fondos de Reserva"
            />
            <p class="text-sm text-gray-500 ml-6">
              Solo aplica para empleados con más de 1 año de antigüedad (8.33% del salario)
            </p>
            <p v-if="errors.is_FondoReserva" class="text-sm text-red-600 ml-6">
              {{ errors.is_FondoReserva }}
            </p>
          </div>
        </div>
      </div>

      <!-- Info Box -->
      <div class="bg-blue-50 border border-blue-200 rounded-lg p-4">
        <h3 class="font-medium text-blue-800">Información importante</h3>
        <ul class="mt-2 text-sm text-blue-700 list-disc list-inside space-y-1">
          <li>El ID del empleado se genera automáticamente</li>
          <li>La fecha de creación se registra automáticamente</li>
          <li>Los fondos de reserva solo aplican después de 1 año de trabajo</li>
          <li>El salario base es usado para calcular todas las prestaciones</li>
        </ul>
      </div>

      <!-- Actions -->
      <div class="flex justify-end space-x-3">
        <BaseButton variant="outline" @click="goBack"> Cancelar </BaseButton>
        <BaseButton type="submit" variant="primary" :loading="saving">
          {{ isEdit ? 'Actualizar' : 'Crear' }} Empleado
        </BaseButton>
      </div>
    </form>
  </div>
</template>

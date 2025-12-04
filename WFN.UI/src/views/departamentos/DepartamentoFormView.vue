<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { api } from '@/api'
import { useUIStore } from '@/stores'
import type { DepartamentoCreateDTO } from '@/types'
import FormInput from '@/components/forms/FormInput.vue'
import BaseButton from '@/components/common/BaseButton.vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

const router = useRouter()
const route = useRoute()
const uiStore = useUIStore()

const isEdit = computed(() => !!route.params.id)
const pageTitle = computed(() => (isEdit.value ? 'Editar Departamento' : 'Nuevo Departamento'))

const loading = ref(false)
const saving = ref(false)
const errors = ref<Record<string, string>>({})

const form = ref<DepartamentoCreateDTO>({
  nombre: '',
  ubicacion: '',
  email: '',
  telefono: '',
  cargo: '',
  centroCosto: '',
})

async function loadDepartamento() {
  if (!isEdit.value) return

  loading.value = true
  try {
    const departamento = await api.departamento.getById(route.params.id as string)
    form.value = {
      nombre: departamento.nombre,
      ubicacion: departamento.ubicacion,
      email: departamento.email,
      telefono: departamento.telefono,
      cargo: departamento.cargo,
      centroCosto: departamento.centroCosto,
    }
  } catch (error) {
    console.error('Error loading departamento:', error)
    uiStore.notifyError('Error', 'No se pudo cargar el departamento')
    router.push('/departamentos')
  } finally {
    loading.value = false
  }
}

function validate(): boolean {
  errors.value = {}

  if (!form.value.nombre) errors.value.nombre = 'El nombre es requerido'

  return Object.keys(errors.value).length === 0
}

async function handleSubmit() {
  if (!validate()) return

  saving.value = true
  try {
    if (isEdit.value) {
      await api.departamento.update(route.params.id as string, {
        ...form.value,
        id_Departamento: route.params.id as string,
      })
      uiStore.notifySuccess('Éxito', 'Departamento actualizado correctamente')
    } else {
      await api.departamento.create(form.value)
      uiStore.notifySuccess('Éxito', 'Departamento creado correctamente')
    }
    router.push('/departamentos')
  } catch (error) {
    console.error('Error saving departamento:', error)
    uiStore.notifyError('Error', 'No se pudo guardar el departamento')
  } finally {
    saving.value = false
  }
}

function goBack() {
  router.push('/departamentos')
}

onMounted(() => {
  loadDepartamento()
})
</script>

<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">{{ pageTitle }}</h1>
        <p class="mt-1 text-sm text-gray-500">
          {{ isEdit ? 'Modifique los datos del departamento' : 'Complete los datos del nuevo departamento' }}
        </p>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="flex justify-center py-12">
      <LoadingSpinner size="lg" />
    </div>

    <!-- Form -->
    <form v-else class="space-y-6" @submit.prevent="handleSubmit">
      <div class="card">
        <div class="card-header">
          <h2 class="text-lg font-medium text-gray-900">Información del Departamento</h2>
        </div>
        <div class="card-body">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <FormInput
              v-model="form.nombre"
              label="Nombre del Departamento"
              placeholder="Recursos Humanos"
              required
              :error="errors.nombre"
            />
            <FormInput
              v-model="form.ubicacion"
              label="Ubicación"
              placeholder="Edificio A, Piso 2"
            />
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mt-4">
            <FormInput
              v-model="form.cargo"
              label="Cargo Responsable"
              placeholder="Gerente de RRHH"
            />
            <FormInput
              v-model="form.centroCosto"
              label="Centro de Costo"
              placeholder="CC001"
            />
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mt-4">
            <FormInput
              v-model="form.email"
              type="email"
              label="Email de Contacto"
              placeholder="rrhh@empresa.com"
            />
            <FormInput
              v-model="form.telefono"
              type="tel"
              label="Teléfono"
              placeholder="022123456"
            />
          </div>
        </div>
      </div>

      <!-- Actions -->
      <div class="flex justify-end space-x-3">
        <BaseButton variant="outline" @click="goBack"> Cancelar </BaseButton>
        <BaseButton type="submit" variant="primary" :loading="saving">
          {{ isEdit ? 'Actualizar' : 'Crear' }} Departamento
        </BaseButton>
      </div>
    </form>
  </div>
</template>

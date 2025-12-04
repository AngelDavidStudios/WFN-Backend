<script setup lang="ts">
import { computed } from 'vue'
import { Bar } from 'vue-chartjs'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend,
} from 'chart.js'

ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend)

interface Dataset {
  label: string
  data: number[]
  backgroundColor?: string
  borderColor?: string
}

interface Props {
  labels: string[]
  datasets: Dataset[]
}

const props = defineProps<Props>()

const chartData = computed(() => ({
  labels: props.labels,
  datasets: props.datasets.map((dataset) => ({
    ...dataset,
    backgroundColor: dataset.backgroundColor || '#6366f1',
    borderColor: dataset.borderColor || '#4f46e5',
    borderWidth: 1,
  })),
}))

const chartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: true,
      position: 'top' as const,
    },
  },
  scales: {
    y: {
      beginAtZero: true,
      ticks: {
        callback: function(value: any) {
          return '$' + value.toLocaleString()
        }
      }
    },
  },
}))
</script>

<template>
  <div class="relative h-full">
    <Bar :data="chartData" :options="chartOptions" />
  </div>
</template>


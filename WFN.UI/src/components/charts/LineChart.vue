<script setup lang="ts">
import { computed } from 'vue'
import { Line } from 'vue-chartjs'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler,
} from 'chart.js'

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler
)

interface Props {
  labels: string[]
  data: number[]
  label?: string
  borderColor?: string
  backgroundColor?: string
}

const props = withDefaults(defineProps<Props>(), {
  label: 'Dataset',
  borderColor: '#6366f1',
  backgroundColor: 'rgba(99, 102, 241, 0.1)',
})

const chartData = computed(() => ({
  labels: props.labels,
  datasets: [
    {
      label: props.label,
      data: props.data,
      borderColor: props.borderColor,
      backgroundColor: props.backgroundColor,
      tension: 0.4,
      fill: true,
      pointRadius: 4,
      pointHoverRadius: 6,
    },
  ],
}))

const chartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: true,
      position: 'top' as const,
    },
    tooltip: {
      mode: 'index' as const,
      intersect: false,
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
    <Line :data="chartData" :options="chartOptions" />
  </div>
</template>


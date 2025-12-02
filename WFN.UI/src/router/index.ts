import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '@/stores'

// Lazy load components
const MainLayout = () => import('@/components/layout/MainLayout.vue')
const LoginView = () => import('@/views/auth/LoginView.vue')
const DashboardView = () => import('@/views/dashboard/DashboardView.vue')

// Personas
const PersonaListView = () => import('@/views/personas/PersonaListView.vue')
const PersonaDetailView = () => import('@/views/personas/PersonaDetailView.vue')
const PersonaFormView = () => import('@/views/personas/PersonaFormView.vue')

// Empleados
const EmpleadoListView = () => import('@/views/empleados/EmpleadoListView.vue')
const EmpleadoFormView = () => import('@/views/empleados/EmpleadoFormView.vue')

// Departamentos
const DepartamentoListView = () => import('@/views/departamentos/DepartamentoListView.vue')
const DepartamentoFormView = () => import('@/views/departamentos/DepartamentoFormView.vue')

// Banking
const BankingListView = () => import('@/views/banking/BankingListView.vue')
const BankingDetailView = () => import('@/views/banking/BankingDetailView.vue')
const BankingFormView = () => import('@/views/banking/BankingFormView.vue')

// Nóminas (Workspaces → Nóminas → Novedades)
const NominaWorkspaceListView = () => import('@/views/nominas/NominaWorkspaceListView.vue')
const NominaPeriodoListView = () => import('@/views/nominas/NominaPeriodoListView.vue')
const NominaDetailView = () => import('@/views/nominas/NominaDetailView.vue')
const NovedadListView = () => import('@/views/novedades/NovedadListView.vue')

const routes: RouteRecordRaw[] = [
  // Public routes
  {
    path: '/login',
    name: 'login',
    component: LoginView,
    meta: { public: true },
  },

  // Protected routes with MainLayout
  {
    path: '/',
    component: MainLayout,
    children: [
      {
        path: '',
        redirect: '/dashboard',
      },
      {
        path: 'dashboard',
        name: 'dashboard',
        component: DashboardView,
        meta: { module: 'dashboard' },
      },

      // Personas routes
      {
        path: 'personas',
        name: 'personas',
        component: PersonaListView,
        meta: { module: 'personas' },
      },
      {
        path: 'personas/nuevo',
        name: 'persona-create',
        component: PersonaFormView,
        meta: { module: 'personas' },
      },
      {
        path: 'personas/:id',
        name: 'persona-view',
        component: PersonaDetailView,
        meta: { module: 'personas' },
      },
      {
        path: 'personas/:id/editar',
        name: 'persona-edit',
        component: PersonaFormView,
        meta: { module: 'personas' },
      },

      // Empleados routes
      {
        path: 'empleados',
        name: 'empleados',
        component: EmpleadoListView,
        meta: { module: 'empleados' },
      },
      {
        path: 'empleados/nuevo',
        name: 'empleado-create',
        component: EmpleadoFormView,
        meta: { module: 'empleados' },
      },
      {
        path: 'empleados/:id',
        name: 'empleado-view',
        component: EmpleadoFormView,
        meta: { module: 'empleados' },
      },
      {
        path: 'empleados/:id/editar',
        name: 'empleado-edit',
        component: EmpleadoFormView,
        meta: { module: 'empleados' },
      },

      // Departamentos routes
      {
        path: 'departamentos',
        name: 'departamentos',
        component: DepartamentoListView,
        meta: { module: 'departamentos' },
      },
      {
        path: 'departamentos/nuevo',
        name: 'departamento-create',
        component: DepartamentoFormView,
        meta: { module: 'departamentos' },
      },
      {
        path: 'departamentos/:id/editar',
        name: 'departamento-edit',
        component: DepartamentoFormView,
        meta: { module: 'departamentos' },
      },

      // Nóminas routes (Workspaces → Nóminas → Novedades)
      {
        path: 'nominas',
        name: 'nominas',
        component: NominaWorkspaceListView,
        meta: { module: 'nominas' },
      },
      {
        path: 'nominas/periodo/:periodo',
        name: 'nominas-periodo',
        component: NominaPeriodoListView,
        meta: { module: 'nominas' },
      },
      {
        path: 'nominas/periodo/:periodo/empleado/:empleadoId',
        name: 'nomina-detail',
        component: NominaDetailView,
        meta: { module: 'nominas' },
      },
      {
        path: 'nominas/periodo/:periodo/empleado/:empleadoId/novedades',
        name: 'novedades-empleado',
        component: () => import('@/views/nominas/NovedadNominaView.vue'),
        meta: { module: 'nominas' },
      },

      // Banking routes (Cuentas Bancarias)
      {
        path: 'banking/persona/:personaId',
        name: 'banking-list',
        component: BankingListView,
        meta: { module: 'banking' },
      },
      {
        path: 'banking/persona/:personaId/nuevo',
        name: 'banking-create',
        component: BankingFormView,
        meta: { module: 'banking' },
      },
      {
        path: 'banking/persona/:personaId/:bankId',
        name: 'banking-view',
        component: BankingDetailView,
        meta: { module: 'banking' },
      },
      {
        path: 'banking/persona/:personaId/:bankId/editar',
        name: 'banking-edit',
        component: BankingFormView,
        meta: { module: 'banking' },
      },

      // Provisiones routes (placeholder)
      {
        path: 'provisiones',
        name: 'provisiones',
        component: () => import('@/views/PlaceholderView.vue'),
        meta: { module: 'provisiones', title: 'Provisiones' },
      },

      // Parámetros routes
      {
        path: 'parametros',
        name: 'parametros',
        component: () => import('@/views/parametros/ParametroListView.vue'),
        meta: { module: 'parametros' },
      },


      // Reportes routes (placeholder)
      {
        path: 'reportes',
        name: 'reportes',
        component: () => import('@/views/PlaceholderView.vue'),
        meta: { module: 'reportes', title: 'Reportes' },
      },

      // Administración routes (placeholder)
      {
        path: 'administracion',
        name: 'administracion',
        component: () => import('@/views/PlaceholderView.vue'),
        meta: { module: 'administracion', title: 'Administración', requiresSuperAdmin: true },
      },
    ],
  },

  // 404 redirect
  {
    path: '/:pathMatch(.*)*',
    redirect: '/dashboard',
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

// Navigation guard
router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore()

  // Only check session once, when the app first loads or when navigating from login
  // Don't check on every navigation to avoid repeated API calls
  const shouldCheckSession =
    !authStore.user &&
    !authStore.loading &&
    (from.name === undefined || from.name === 'login')

  if (shouldCheckSession) {
    await authStore.checkSession()
  }

  const isPublicRoute = to.meta.public === true
  const requiresSuperAdmin = to.meta.requiresSuperAdmin === true

  // Allow public routes
  if (isPublicRoute) {
    // If already authenticated, redirect to dashboard
    if (authStore.isAuthenticated) {
      next({ name: 'dashboard' })
      return
    }
    next()
    return
  }

  // Require authentication for protected routes
  if (!authStore.isAuthenticated) {
    next({ name: 'login' })
    return
  }

  // Check super admin requirement
  if (requiresSuperAdmin && !authStore.isSuperAdmin) {
    next({ name: 'dashboard' })
    return
  }

  next()
})

// Error handler para capturar errores de navegación
router.onError((error) => {
  console.error('Router navigation error:', error)
  // Si hay un error de navegación, redirigir al dashboard
  if (!router.currentRoute.value.matched.length) {
    router.push('/dashboard')
  }
})

export default router

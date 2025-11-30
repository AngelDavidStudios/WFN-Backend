import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '@/stores'

// Lazy load components
const MainLayout = () => import('@/components/layout/MainLayout.vue')
const LoginView = () => import('@/views/auth/LoginView.vue')
const DashboardView = () => import('@/views/dashboard/DashboardView.vue')

// Personas
const PersonaListView = () => import('@/views/personas/PersonaListView.vue')
const PersonaFormView = () => import('@/views/personas/PersonaFormView.vue')

// Empleados
const EmpleadoListView = () => import('@/views/empleados/EmpleadoListView.vue')
const EmpleadoFormView = () => import('@/views/empleados/EmpleadoFormView.vue')

// Departamentos
const DepartamentoListView = () => import('@/views/departamentos/DepartamentoListView.vue')
const DepartamentoFormView = () => import('@/views/departamentos/DepartamentoFormView.vue')

// Nóminas
const NominaListView = () => import('@/views/nominas/NominaListView.vue')

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
        component: PersonaFormView,
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

      // Nóminas routes
      {
        path: 'nominas',
        name: 'nominas',
        component: NominaListView,
        meta: { module: 'nominas' },
      },

      // Placeholder routes for other modules
      {
        path: 'novedades',
        name: 'novedades',
        component: () => import('@/views/PlaceholderView.vue'),
        meta: { module: 'novedades', title: 'Novedades' },
      },
      {
        path: 'banking',
        name: 'banking',
        component: () => import('@/views/PlaceholderView.vue'),
        meta: { module: 'banking', title: 'Banking' },
      },
      {
        path: 'provisiones',
        name: 'provisiones',
        component: () => import('@/views/PlaceholderView.vue'),
        meta: { module: 'provisiones', title: 'Provisiones' },
      },
      {
        path: 'parametros',
        name: 'parametros',
        component: () => import('@/views/PlaceholderView.vue'),
        meta: { module: 'parametros', title: 'Parámetros' },
      },
      {
        path: 'workspaces',
        name: 'workspaces',
        component: () => import('@/views/PlaceholderView.vue'),
        meta: { module: 'workspaces', title: 'Workspaces' },
      },
      {
        path: 'reportes',
        name: 'reportes',
        component: () => import('@/views/PlaceholderView.vue'),
        meta: { module: 'reportes', title: 'Reportes' },
      },
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
router.beforeEach(async (to, _from, next) => {
  const authStore = useAuthStore()

  // Check session if not already loaded
  if (!authStore.user && !authStore.loading) {
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

export default router

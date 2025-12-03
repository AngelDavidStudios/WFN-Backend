import type { User, Session } from '@supabase/supabase-js'

// Role types
export interface Role {
  id: string
  name: string
  display_name: string
  description: string
  is_system_role?: boolean
  created_at?: string
}

export type RoleName = 'SUPER_ADMIN' | 'ADMIN' | 'RRHH' | 'EMPLEADO'

// User profile
export interface UserProfile {
  id: string
  email: string
  first_name: string
  last_name: string
  role_id: string
  is_active: boolean
  created_at?: string
  updated_at?: string
  roles?: Role // For joined queries
}

// Module permissions
export interface Module {
  id: string
  name: ModuleName
  display_name: string
  route: string
  icon?: string
}

export type ModuleName =
  | 'dashboard'
  | 'personas'
  | 'empleados'
  | 'departamentos'
  | 'nominas'
  | 'novedades'
  | 'banking'
  | 'provisiones'
  | 'parametros'
  | 'workspaces'
  | 'reportes'
  | 'administracion'
  | 'roles'
  | 'usuarios'

export interface RoleModulePermission {
  role_id: string
  module_id: string
  can_view: boolean
  can_create: boolean
  can_edit: boolean
  can_delete: boolean
}

// Extended permission with module info
export interface ModulePermission extends RoleModulePermission {
  module_name: ModuleName
  module_display_name: string
  module_route: string
}

// Auth state
export interface AuthState {
  user: User | null
  session: Session | null
  userProfile: UserProfile | null
  userRole: Role | null
  permissions: ModulePermission[]
  loading: boolean
  error: string | null
}

// Login credentials
export interface LoginCredentials {
  email: string
  password: string
}

// Register user data
export interface RegisterUserData {
  email: string
  password: string
  first_name: string
  last_name: string
  role_id: string
}

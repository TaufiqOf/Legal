export interface User {
  id?: string;
  name: string;
  userName: string;
  email?: string;
}

export interface LoginRequest {
  userName: string;
  password: string;
  moduleName: number;
}

export interface RegistrationRequest {
  name: string;
  userName: string;
  password: string;
  moduleName: number;
}

export interface AuthResponse {
  success: boolean;
  result: {
    user: User;
    token: string;
  };
  message?: string;
  errors?: string[];
}

export interface JwtPayload {
  sub: string;
  name: string;
  userName: string;
  exp: number;
  iat: number;
}

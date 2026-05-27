import { Policy } from './policy.model';

export interface PolicyHolder {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  dateOfBirth: string; // YYYY-MM-DD
  regionCode: string;
  createdAt: string;
  updatedAt: string;
  
  // Navigation property
  policies?: Policy[];
}

export interface CreatePolicyHolderRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  dateOfBirth: string;
  regionCode: string;
}

export interface UpdatePolicyHolderRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  dateOfBirth: string;
  regionCode: string;
}

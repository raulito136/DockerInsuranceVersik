export interface Policy {
  id: number;
  policyNumber: string;
  policyHolderId: number;
  policyTypeCode: string;
  coverageTypeCode: string;
  coverageAmount: number;
  premiumAmount: number;
  startDate: string; // YYYY-MM-DD
  endDate: string; // YYYY-MM-DD
  status: 'ACTIVE' | 'INACTIVE' | 'EXPIRED' | 'CANCELLED';
  createdAt: string;
  updatedAt: string;
  
  // Optional navigation property for when API includes the holder
  policyHolder?: any; // We'll import PolicyHolder when both exist to avoid circular dependency initially if needed, or just define it.
}

export interface CreatePolicyRequest {
  policyHolderId: number;
  policyTypeCode: string;
  coverageTypeCode: string;
  coverageAmount: number;
  premiumAmount: number;
  startDate: string;
  endDate: string;
}

export interface UpdatePolicyRequest {
  policyTypeCode: string;
  coverageTypeCode: string;
  coverageAmount: number;
  premiumAmount: number;
  startDate: string;
  endDate: string;
  status: 'ACTIVE' | 'INACTIVE' | 'EXPIRED' | 'CANCELLED';
}

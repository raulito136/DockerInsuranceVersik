import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ClaimStatus } from '../models/claim-status.model';
import { ApiResponse, PagedResponse } from '../../../../shared-models/src/lib/shared-models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class ClaimStatusService {
  private http = inject(HttpClient);
  private apiUrl = 'http://172.22.76.32:5001/api/v1/claim-statuses'; 

  getClaimStatuses(includeInactive: boolean = false) {
    return this.http.get<PagedResponse<ClaimStatus>>(`${this.apiUrl}?includeInactive=${includeInactive}`);
  }

  getClaimStatusById(id: number) {
    return this.http.get<ApiResponse<ClaimStatus>>(`${this.apiUrl}/${id}`);
  }

  getClaimStatusByCode(code: string) {
    return this.http.get<ApiResponse<ClaimStatus>>(`${this.apiUrl}/by-code/${code}`);
  }

  createClaimStatus(claimStatus: ClaimStatus) {
    return this.http.post<ApiResponse<ClaimStatus>>(this.apiUrl, claimStatus);
  }

  updateClaimStatus(id: number, claimStatus: ClaimStatus) {
    return this.http.put<ApiResponse<ClaimStatus>>(`${this.apiUrl}/${id}`, claimStatus);
  }

  deleteClaimStatus(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  activateClaimStatus(id: number) {
    return this.http.put<ApiResponse<ClaimStatus>>(`${this.apiUrl}/${id}/activate`, {});
  }
}
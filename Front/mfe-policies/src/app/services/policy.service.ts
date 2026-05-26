import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResponse, ApiResponse } from '@policy-system/shared-models';
import { Policy, CreatePolicyRequest, UpdatePolicyRequest } from '../models/policy.model';

@Injectable({
  providedIn: 'root'
})
export class PolicyService {
  private http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5002/api/v1/policies';

  getPolicies(page: number = 1, pageSize: number = 20, status?: string, policyTypeCode?: string): Observable<PagedResponse<Policy>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (status) {
      params = params.set('status', status);
    }
    if (policyTypeCode) {
      params = params.set('policyTypeCode', policyTypeCode);
    }

    return this.http.get<PagedResponse<Policy>>(this.baseUrl, { params });
  }

  getPolicyById(id: number): Observable<ApiResponse<Policy>> {
    return this.http.get<ApiResponse<Policy>>(`${this.baseUrl}/${id}`);
  }

  createPolicy(request: CreatePolicyRequest): Observable<ApiResponse<Policy>> {
    return this.http.post<ApiResponse<Policy>>(this.baseUrl, request);
  }

  updatePolicy(id: number, request: UpdatePolicyRequest): Observable<ApiResponse<Policy>> {
    return this.http.put<ApiResponse<Policy>>(`${this.baseUrl}/${id}`, request);
  }

  deletePolicy(id: number): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.baseUrl}/${id}`);
  }
}

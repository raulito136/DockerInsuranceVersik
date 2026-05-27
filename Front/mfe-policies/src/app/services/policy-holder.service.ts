import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResponse, ApiResponse } from '@policy-system/shared-models';
import { PolicyHolder, CreatePolicyHolderRequest, UpdatePolicyHolderRequest } from '../models/policy-holder.model';

@Injectable({
  providedIn: 'root'
})
export class PolicyHolderService {
  private http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5002/api/v1/policy-holders';

  getPolicyHolders(page: number = 1, pageSize: number = 20): Observable<PagedResponse<PolicyHolder>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<PagedResponse<PolicyHolder>>(this.baseUrl, { params });
  }

  getPolicyHolderById(id: number): Observable<ApiResponse<PolicyHolder>> {
    return this.http.get<ApiResponse<PolicyHolder>>(`${this.baseUrl}/${id}`);
  }

  createPolicyHolder(request: CreatePolicyHolderRequest): Observable<ApiResponse<PolicyHolder>> {
    return this.http.post<ApiResponse<PolicyHolder>>(this.baseUrl, request);
  }

  updatePolicyHolder(id: number, request: UpdatePolicyHolderRequest): Observable<ApiResponse<PolicyHolder>> {
    return this.http.put<ApiResponse<PolicyHolder>>(`${this.baseUrl}/${id}`, request);
  }

  deletePolicyHolder(id: number): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.baseUrl}/${id}`);
  }
}

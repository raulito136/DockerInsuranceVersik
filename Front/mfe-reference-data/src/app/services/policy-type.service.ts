import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { PolicyType } from '../models/policy-type.model';
import { ApiResponse, PagedResponse } from '../../../../shared-models/src/lib/shared-models/api-response.model';


@Injectable({
  providedIn: 'root'
})
export class PolicyTypeService {
  private http=inject(HttpClient);
  private apiUrl="http://localhost:5003/api/v1/policy-types";

  getPolicyTypes(includeInactive: boolean = false){
    return this.http.get<PagedResponse<PolicyType>>(`${this.apiUrl}?includeInactive=${includeInactive}`);
  }

  getPolicyTypeById(id:number){
    return this.http.get<ApiResponse<PolicyType>>(`${this.apiUrl}/${id}`);
  }

  getPolicyTypeByCode(code:string){
    return this.http.get<ApiResponse<PolicyType>>(`${this.apiUrl}/by-code/${code}`);
  }

  createPolicyType(item:PolicyType){
    return this.http.post<ApiResponse<PolicyType>>(this.apiUrl,item);
  }

  updatePolicyType(id:number,item:PolicyType){
    return this.http.put<ApiResponse<PolicyType>>(`${this.apiUrl}/${id}`,item);
  }

  deletePolicyType(id:number){
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  activatePolicyType(id: number) {
    return this.http.put<ApiResponse<PolicyType>>(`${this.apiUrl}/${id}/activate`, {});
  }
}

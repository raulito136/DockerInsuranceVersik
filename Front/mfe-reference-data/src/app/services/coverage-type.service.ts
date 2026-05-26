import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CoverageType } from '../models/coverage-type.model';
import { ApiResponse, PagedResponse } from '../../../../shared-models/src/lib/shared-models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class CoverageTypeService {
  private http=inject(HttpClient);
  private apiUrl = 'http://localhost:5003/api/v1/coverage-types';
  
  
  getCoverageTypes(includeInactive: boolean = false){
    return this.http.get<PagedResponse<CoverageType>>(`${this.apiUrl}?includeInactive=${includeInactive}`);
  }

  getCoverageTypeById(id:number){
    return this.http.get<ApiResponse<CoverageType>>(`${this.apiUrl}/${id}`);
  }

  getCoverageTypeByCode(code:string){
    return this.http.get<ApiResponse<CoverageType>>(`${this.apiUrl}/by-code/${code}`);
  }

  createCoverageType(item:CoverageType){
    return this.http.post<ApiResponse<CoverageType>>(this.apiUrl,item);
  }

  updateCoverageType(id:number,item:CoverageType){
    return this.http.put<ApiResponse<CoverageType>>(`${this.apiUrl}/${id}`,item);
  }

  deleteCoverageType(id:number){
      return this.http.delete(`${this.apiUrl}/${id}`);
  }

  activateCoverageType(id: number) {
    return this.http.put<ApiResponse<CoverageType>>(`${this.apiUrl}/${id}/activate`, {});
  }
}

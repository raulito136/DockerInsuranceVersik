import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Region } from '../models/region.model';
import { ApiResponse, PagedResponse } from '../../../../shared-models/src/lib/shared-models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class RegionService {
  private http=inject(HttpClient);
  private apiUrl="http://172.22.76.32:5001/api/v1/regions";

  getRegions(includeInactive: boolean = false){
    return this.http.get<PagedResponse<Region>>(`${this.apiUrl}?includeInactive=${includeInactive}`);
  }

  getRegionsById(id:number){
    return this.http.get<ApiResponse<Region>>(`${this.apiUrl}/${id}`);
  }

  createRegion(item:Region){
    return this.http.post<ApiResponse<Region>>(this.apiUrl,item);
  }

  updateRegion(id:number,item:Region){
    return this.http.put<ApiResponse<Region>>(`${this.apiUrl}/${id}`,item);
  }

  deleteRegion(id:number){
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

   activateRegion(id: number) {
      return this.http.put<ApiResponse<Region>>(`${this.apiUrl}/${id}/activate`, {});
    }
}

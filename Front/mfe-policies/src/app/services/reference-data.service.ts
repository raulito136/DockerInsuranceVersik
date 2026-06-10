import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { ApiResponse } from '@policy-system/shared-models';

export interface ReferenceDataEntity {
  code: string;
  name: string;
  description?: string;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ReferenceDataService {
  private http = inject(HttpClient);
  private readonly baseUrl = 'http://172.22.76.32:5001/api/v1';

  getPolicyTypes(): Observable<ReferenceDataEntity[]> {
    return this.http.get<ApiResponse<ReferenceDataEntity[]>>(`${this.baseUrl}/policy-types`)
      .pipe(map(response => response.data || []));
  }

  getCoverageTypes(): Observable<ReferenceDataEntity[]> {
    return this.http.get<ApiResponse<ReferenceDataEntity[]>>(`${this.baseUrl}/coverage-types`)
      .pipe(map(response => response.data || []));
  }

  getRegions(): Observable<ReferenceDataEntity[]> {
    return this.http.get<ApiResponse<ReferenceDataEntity[]>>(`${this.baseUrl}/regions`)
      .pipe(map(response => response.data || []));
  }
}

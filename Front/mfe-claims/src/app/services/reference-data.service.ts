import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';

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
  private readonly baseUrl = 'http://localhost:5003/api/v1';

  getClaimStatuses(): Observable<ReferenceDataEntity[]> {
    return this.http.get<any>(`${this.baseUrl}/claim-statuses`)
      .pipe(map(response => response.data || []));
  }
}

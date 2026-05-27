import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ClaimService {
  constructor(private http: HttpClient) { }

  private apiUrl = 'http://localhost:5001/api/v1/claims';
  private useMock = false; // Cambiar a false para usar la API real

  private mockClaims = [
    { id: 1, claimNumber: 'CLM-001', policyNumber: 'POL-123', statusCode: 'PENDING', amount: 1500, description: 'Accidente de tráfico leve', claimDate: '2024-05-01' },
    { id: 2, claimNumber: 'CLM-002', policyNumber: 'POL-456', statusCode: 'APPROVED', amount: 3200, description: 'Rotura de tubería en cocina', claimDate: '2024-05-05' },
    { id: 3, claimNumber: 'CLM-003', policyNumber: 'POL-789', statusCode: 'REJECTED', amount: 500, description: 'Robo de bicicleta (sin candado)', claimDate: '2024-05-10' }
  ];

  getAllClaims(page = 1, pageSize = 20, statusCode?: string, policyNumber?: string): Observable<any> {
    if (this.useMock) {
      let filtered = [...this.mockClaims];
      if (statusCode) filtered = filtered.filter(c => c.statusCode === statusCode);
      if (policyNumber) filtered = filtered.filter(c => c.policyNumber.includes(policyNumber));
      return of({ data: filtered, totalCount: filtered.length });
    }

    let url = `${this.apiUrl}?page=${page}&pageSize=${pageSize}`;
    if (statusCode) url += `&statusCode=${statusCode}`;
    if (policyNumber) url += `&policyNumber=${policyNumber}`;

    return this.http.get(url);
  }

  getClaimById(id: number): Observable<any> {
    if (this.useMock) {
      const claim = this.mockClaims.find(c => c.id === +id);
      return of({ data: claim });
    }
    return this.http.get(`${this.apiUrl}/${id}`);
  }

  createClaim(request: any): Observable<any> {
    if (this.useMock) {
      const newClaim = { ...request, id: this.mockClaims.length + 1, claimNumber: `CLM-00${this.mockClaims.length + 1}`, statusCode: 'PENDING' };
      this.mockClaims.push(newClaim);
      return of({ data: newClaim });
    }
    return this.http.post(this.apiUrl, request);
  }

  updateStatus(id: number, newStatus: string, changedBy: string): Observable<any> {
    if (this.useMock) {
      const claim = this.mockClaims.find(c => c.id === +id);
      if (claim) claim.statusCode = newStatus;
      return of({ success: true });
    }
    return this.http.patch(`${this.apiUrl}/${id}/status`, { statusCode: newStatus, changedBy: changedBy });
  }
  deleteClaim(id: number): Observable<any> {
    if (this.useMock) {
      const index = this.mockClaims.findIndex(c => c.id === +id);
      if (index !== -1) this.mockClaims.splice(index, 1);
      return of({ success: true });
    }
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
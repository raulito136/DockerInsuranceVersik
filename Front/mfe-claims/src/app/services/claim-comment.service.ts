import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ClaimCommentService {
  constructor(private http: HttpClient) {}
  
  private apiUrl = 'http://172.22.76.32:5003/api/v1/claims';
  private useMock = false;

  private mockComments: any[] = [];

  addComment(claimId: number, authorName: string, comment: string): Observable<any> {
    if (this.useMock) {
      const newComment = { id: Math.floor(Math.random() * 1000), claimId, authorName, comment, date: new Date().toISOString() };
      this.mockComments.push(newComment);
      return of({ data: newComment });
    }
    return this.http.post(`${this.apiUrl}/${claimId}/comments`, { authorName, comment });
  }

  deleteComment(claimId: number, commentId: number): Observable<any> {
    if (this.useMock) {
      this.mockComments = this.mockComments.filter(c => c.id !== commentId);
      return of({ success: true });
    }
    return this.http.delete(`${this.apiUrl}/${claimId}/comments/${commentId}`);
  }
}
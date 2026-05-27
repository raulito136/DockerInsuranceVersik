import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ClaimComment } from '../../../models/claim-comment.model';
import { CommentFormComponent } from '../comment-form/comment-form.component';
import { TimelineComponent, TimelineEvent } from '@policy-system/ui';

@Component({
  selector: 'app-comment-list',
  standalone: true,
  imports: [CommonModule, CommentFormComponent, TimelineComponent],
  template: `
    <div class="mb-6">
      <h3 class="text-xl font-bold text-gray-800 mb-4">Comments</h3>
      
      <div *ngIf="comments.length > 0; else noComments">
        <lib-timeline [events]="timelineEvents"></lib-timeline>
      </div>
      
      <ng-template #noComments>
        <p class="text-gray-500 italic py-4">No comments yet.</p>
      </ng-template>
    </div>

    <app-comment-form [claimId]="claimId" (commentAdded)="onCommentAdded()"></app-comment-form>
  `
})
export class CommentListComponent {
  @Input() comments: ClaimComment[] = [];
  @Input() claimId!: number;
  @Output() commentsUpdated = new EventEmitter<void>();

  get timelineEvents(): TimelineEvent[] {
    return this.comments.map(c => ({
      title: c.authorName,
      timestamp: c.createdAt,
      description: c.comment,
      status: 'info'
    }));
  }

  onCommentAdded() {
    this.commentsUpdated.emit();
  }
}
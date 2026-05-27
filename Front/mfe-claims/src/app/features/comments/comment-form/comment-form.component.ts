import { Component, inject, Input, Output, EventEmitter, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClaimCommentService } from '../../../services/claim-comment.service';
import { InputComponent, TextareaComponent, ButtonComponent, ConfirmationComponent } from '@policy-system/ui';

@Component({
  selector: 'app-comment-form',
  standalone: true,
  imports: [CommonModule, FormsModule, InputComponent, TextareaComponent, ButtonComponent, ConfirmationComponent],
  template: `
    <div class="mt-6 border-t pt-6">
      <h3 class="text-lg font-bold text-gray-800 mb-4">Add a Comment</h3>
      <div class="grid grid-cols-1 gap-4">
        <lib-input label="Author" [(ngModel)]="author" placeholder="Your name"></lib-input>
        <lib-textarea label="Comment" [(ngModel)]="text" placeholder="Type your comment here..."></lib-textarea>
        <div class="flex justify-end">
          <lib-button type="confirm" content="Post Comment" (buttonClick)="agregar()"></lib-button>
        </div>
      </div>
    </div>

    <lib-confirmation 
      *ngIf="showModal()" 
      [title]="modalTitle()" 
      [text]="modalText()" 
      (confirmAction)="onModalClose()">
    </lib-confirmation>
  `
})
export class CommentFormComponent {
  private commentService = inject(ClaimCommentService);
  @Input() claimId!: number;
  @Output() commentAdded = new EventEmitter<void>();
  
  author = '';
  text = '';

  showModal = signal(false);
  modalTitle = signal('');
  modalText = signal('');

  agregar() {
    if (!this.author || !this.text) {
      this.showAlert('Validation', 'Please fill in both author and comment fields.');
      return;
    }

    this.commentService.addComment(this.claimId, this.author, this.text).subscribe({
      next: () => {
        this.author = '';
        this.text = '';
        this.commentAdded.emit();
        this.showAlert('Success', 'Comment added successfully.');
      },
      error: (err) => {
        this.showAlert('Error', 'Failed to add comment. Please try again.');
      }
    });
  }

  showAlert(title: string, text: string) {
    this.modalTitle.set(title);
    this.modalText.set(text);
    this.showModal.set(true);
  }

  onModalClose() {
    this.showModal.set(false);
  }
}
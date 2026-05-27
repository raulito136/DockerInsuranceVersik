import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

export type TimelineEventStatus = 'success' | 'warning' | 'danger' | 'info' | 'default';

export interface TimelineEvent {
  title: string;
  timestamp: string;
  description?: string;
  status?: TimelineEventStatus;
}

@Component({
  selector: 'lib-timeline',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './timeline.component.html',
  styleUrl: './timeline.component.css',
})
export class TimelineComponent {
  // Array of events to display in the timeline
  @Input() events: TimelineEvent[] = [];

  // Helper method to get the dot color based on status
  getDotColor(status?: TimelineEventStatus): string {
    switch (status) {
      case 'success':
        return 'bg-green-500 border-green-200';
      case 'warning':
        return 'bg-yellow-500 border-yellow-200';
      case 'danger':
        return 'bg-red-500 border-red-200';
      case 'info':
        return 'bg-blue-500 border-blue-200';
      case 'default':
      default:
        return 'bg-gray-400 border-gray-200';
    }
  }
}

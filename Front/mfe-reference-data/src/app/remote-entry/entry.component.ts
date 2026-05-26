import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';

@Component({
    imports: [CommonModule, RouterOutlet],
    selector: 'app-mfe-reference-data-entry',
    template: `<router-outlet></router-outlet>`
})
export class RemoteEntryComponent {}
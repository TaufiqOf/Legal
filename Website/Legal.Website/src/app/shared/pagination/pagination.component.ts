import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.scss']
})
export class PaginationComponent {
  @Input() currentPage = 1;
  @Input() totalPages = 1;
  @Output() pageChange = new EventEmitter<number>();

  onPageChange(page: number) {
    if (page < 1 || page > this.totalPages || page === this.currentPage) return;
    this.pageChange.emit(page);
  }

  getPaginationArray(): number[] {
    const pages: number[] = [];
    const windowSize = 2; // show current +/- 2 like original
    const start = Math.max(1, this.currentPage - windowSize);
    const end = Math.min(this.totalPages, this.currentPage + windowSize);
    for (let p = start; p <= end; p++) pages.push(p);
    return pages;
  }

  showFirst(): boolean { return this.currentPage > 3; }
  showFirstEllipsis(): boolean { return this.currentPage > 4; }
  showLastEllipsis(): boolean { return this.currentPage < this.totalPages - 3; }
  showLast(): boolean { return this.currentPage < this.totalPages - 2; }
}

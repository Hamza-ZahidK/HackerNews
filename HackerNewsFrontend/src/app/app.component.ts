import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { ApiService } from './services/api.service';
import { HackerNewsItem } from './modelws/hacker-news-item.model';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  @ViewChild('dt1') dt1: any;
  public stories: HackerNewsItem[] = [];
  public loading: boolean = false;
  public  searchValue: string | undefined;
  private apiService = inject(ApiService)

  ngOnInit(): void {
    this.getHackersNewItems()
  }

  getHackersNewItems() {
    this.loading = true;
    this.apiService.getNewestStories().subscribe(
      (data: HackerNewsItem[] | any ) => {
        this.stories = data;                
        this.loading = false;
      },
      error => {
        console.error('Error fetching hacker news items:', error);
        this.loading = false;
      }
    );
  }

  onGlobalSearch(event: Event) {
    const input = event.target as HTMLInputElement;
    this.dt1.filterGlobal(input.value, 'contains');
  }
}

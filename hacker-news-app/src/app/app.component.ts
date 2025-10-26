import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { StoryListComponent } from './components/story-list/story-list.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, StoryListComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'hacker-news-app';
}

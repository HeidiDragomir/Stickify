import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { Card } from '../../core/components/card/card';

@Component({
    selector: 'app-home',
    imports: [Card, RouterLink],
    templateUrl: './home.html',
    styleUrl: './home.css',
})
export class Home {}

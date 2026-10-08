export interface AppNotification{
    id: number;
    type: string;
    message: string;
    relatedBookingId : number | null;
    createdAt: string;
    isRead: boolean;
}
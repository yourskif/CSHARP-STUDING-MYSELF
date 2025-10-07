# Order state datails

## Task

### Order states
Each order in your application will have next states:

- **New Order**
- **Canceled by user**
- **Canceled by administrator**
- **Confirmed**
- **Moved to delivery company**
- **In delivery**
- **Delivered to client**
- **Delivery confirmed by client**


### Allowed Status Changes
You must implement order status changing acording to this image:  
![Order Status Changes](OrderStatusChanges.jpg)  
If the user has selected an incorect new status of order you must keep the last valid state.






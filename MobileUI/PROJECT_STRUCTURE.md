# NoteSprint Mobile UI Project Structure

This document outlines the project structure for the NoteSprint Mobile UI application. The project has been organized to be clean, maintainable, and easy to extend.

## Directory Structure

```
NoteSprint/MobileUI/
├── app/                  # Expo Router app directory
│   ├── (modals)/         # Modal screens
│   ├── (tabs)/           # Tab-based navigation screens
│   ├── _layout.tsx       # Root layout component
│   └── +not-found.tsx    # 404 page
├── assets/               # Static assets like images, fonts
├── src/                  # Main source code directory
│   ├── api/              # API-related code
│   ├── components/       # Reusable UI components
│   ├── constants/        # Application constants
│   ├── context/          # React context providers
│   ├── hooks/            # Custom React hooks
│   ├── navigation/       # Navigation-related code
│   ├── screens/          # Screen components
│   ├── theme/            # Theme-related code
│   ├── types/            # TypeScript type definitions
│   └── utils/            # Utility functions
├── .expo/                # Expo configuration
├── android/              # Android-specific code
├── node_modules/         # Dependencies
└── various config files  # Configuration files for the project
```

## Directory Purposes

### `/app`
Contains the Expo Router configuration and screen components. This follows the file-based routing approach of Expo Router.

### `/assets`
Contains static assets like images, fonts, and other media files used in the application.

### `/src/api`
Houses all API-related code, including:
- API client configuration
- Request/response handling
- API endpoints
- Data transformation logic

### `/src/components`
Contains reusable UI components that are used across multiple screens. Components should be modular and follow a clear naming convention.

### `/src/constants`
Contains application-wide constants, such as:
- Configuration values
- Theme colors
- API endpoints
- Feature flags

### `/src/context`
Contains React context providers for state management across the application.

### `/src/hooks`
Contains custom React hooks that encapsulate reusable logic, such as:
- Data fetching hooks
- UI state hooks
- Device feature hooks

### `/src/navigation`
Contains navigation-related code, including:
- Navigation configuration
- Navigation utilities
- Custom navigation components

### `/src/screens`
Contains screen components that represent full pages in the application. These components typically:
- Compose multiple smaller components
- Handle screen-specific logic
- Connect to API services

### `/src/theme`
Contains theme-related code, including:
- Theme definitions
- Theme utilities
- Theming hooks

### `/src/types`
Contains TypeScript type definitions used throughout the application, including:
- Interface definitions
- Type aliases
- Enum definitions

### `/src/utils`
Contains utility functions that are used across the application, such as:
- Date formatting
- String manipulation
- Validation functions
- Helper functions

## Best Practices

1. **Component Structure**: Each component should be in its own directory with an index.ts file for exporting.
2. **API Organization**: Organize API calls by feature or entity.
3. **Type Safety**: Use TypeScript interfaces and types for all API responses and requests.
4. **Code Splitting**: Split code into logical modules to maintain readability and performance.
5. **State Management**: Use React Context for global state management.
6. **Error Handling**: Implement consistent error handling across API calls.
7. **Testing**: Write tests for critical components and utilities.

## Migration Plan

To migrate from the current structure to the new structure:

1. Move existing components to `/src/components`
2. Move hooks to `/src/hooks`
3. Move constants to `/src/constants`
4. Create API services in `/src/api`
5. Refactor screens into `/src/screens`
6. Update imports across the codebase

## Extending the Project

When adding new features to the project:

1. Create new components in the appropriate directories
2. Add new API endpoints in the `/src/api` directory
3. Create new screens in the `/src/screens` directory
4. Update navigation as needed
5. Add new types to the `/src/types` directory

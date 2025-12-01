# ...existing code...

# ============================================
# Database Migrations
# ============================================
migration:
	dotnet ef migrations add $(name) --project src/Emm.Infrastructure --startup-project src/Emm.Presentation

migration-remove:
	dotnet ef migrations remove --project src/Emm.Infrastructure --startup-project src/Emm.Presentation

migration-update:
	dotnet ef database update --project src/Emm.Infrastructure --startup-project src/Emm.Presentation

# ============================================
# Build & Publish
# ============================================

# Build the application
build:
	dotnet build src/Emm.Presentation/Emm.Presentation.csproj --configuration Release

# Clean build artifacts
clean:
	dotnet clean src/Emm.Presentation/Emm.Presentation.csproj
	rm -rf ./publish

# Publish for development (with debugging symbols)
publish-dev:
	dotnet publish src/Emm.Presentation/Emm.Presentation.csproj \
		--configuration Debug \
		--output ./publish/dev \
		--no-restore

# Publish for production (optimized, no debugging symbols)
.PHONY: publish
publish:
	dotnet publish src/Emm.Presentation/Emm.Presentation.csproj \
		--configuration Release \
		--output ./publish/release \
		--no-restore \
		/p:DebugType=None \
		/p:DebugSymbols=false

# Publish as self-contained (includes .NET runtime)
publish-standalone:
	dotnet publish src/Emm.Presentation/Emm.Presentation.csproj \
		--configuration Release \
		--output ./publish/standalone \
		--runtime win-x64 \
		--self-contained true \
		/p:PublishSingleFile=false \
		/p:DebugType=None

# Publish as single executable file (experimental)
publish-single:
	dotnet publish src/Emm.Presentation/Emm.Presentation.csproj \
		--configuration Release \
		--output ./publish/single \
		--runtime win-x64 \
		--self-contained true \
		/p:PublishSingleFile=true \
		/p:DebugType=None

# Create zip package for manual upload to IIS server
package:
	@echo "Creating deployment package..."
	dotnet publish src/Emm.Presentation/Emm.Presentation.csproj \
		--configuration Release \
		--output ./publish/package \
		--no-restore \
		/p:DebugType=None
	@echo "Creating zip archive..."
	cd ./publish/package && zip -r ../emm-app-$$(date +%Y%m%d-%H%M%S).zip .
	@echo "Package created in ./publish/"

# ============================================
# Tools
# ============================================

# Generate error codes JSON for frontend
error-codes:
	@echo "Generating error codes..."
	dotnet run --project tools/Emm.Tools.ErrorCodeGenerator -- error-codes.json
	@echo "Done! Output: error-codes.json"

# Generate error codes to specific path
error-codes-to:
	@echo "Generating error codes to $(path)..."
	dotnet run --project tools/Emm.Tools.ErrorCodeGenerator -- $(path)
	@echo "Done!"

# ...existing code...

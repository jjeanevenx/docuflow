resource "aws_s3_bucket" "documents" { bucket = "docuflow-documents" }
resource "aws_s3_bucket" "web" { bucket = "docuflow-web" }

resource "aws_s3_bucket_cors_configuration" "documents" {
  bucket = aws_s3_bucket.documents.id
  cors_rule {
    allowed_headers = ["*"]
    allowed_methods = ["PUT", "GET", "HEAD"]
    allowed_origins = ["*"]
    expose_headers  = ["ETag"]
  }
}

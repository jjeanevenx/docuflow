resource "aws_sqs_queue" "inventory" { name = "docuflow-inventory-queue" }
resource "aws_sqs_queue" "zip" { name = "docuflow-zip-queue" }

resource "aws_sns_topic_subscription" "inventory" {
  topic_arn = aws_sns_topic.document_uploaded.arn
  protocol  = "sqs"
  endpoint  = aws_sqs_queue.inventory.arn
}

resource "aws_sns_topic_subscription" "zip" {
  topic_arn = aws_sns_topic.document_uploaded.arn
  protocol  = "sqs"
  endpoint  = aws_sqs_queue.zip.arn
}

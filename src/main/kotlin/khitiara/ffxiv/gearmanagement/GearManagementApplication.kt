package khitiara.ffxiv.gearmanagement

import io.lettuce.core.RedisURI
import org.springframework.beans.factory.annotation.Value
import org.springframework.boot.autoconfigure.SpringBootApplication
import org.springframework.boot.context.properties.EnableConfigurationProperties
import org.springframework.boot.runApplication
import org.springframework.cache.annotation.EnableCaching
import org.springframework.context.annotation.Bean
import org.springframework.data.redis.cache.RedisCacheConfiguration
import org.springframework.data.redis.cache.RedisCacheManager
import org.springframework.data.redis.connection.RedisConnectionFactory
import org.springframework.data.redis.connection.lettuce.LettuceConnectionFactory
import org.springframework.scheduling.annotation.EnableAsync
import org.springframework.web.reactive.function.client.WebClient
import java.time.Duration


@SpringBootApplication
@EnableCaching
@EnableAsync
@EnableConfigurationProperties
class GearManagementApplication {

    @Value("\${spring.redis.url}")
    private val redisUri: String? = null

    @Bean
    fun restTemplate(): WebClient =
        WebClient.create("https://xivapi.com")

    @Bean
    fun redisConnectionFactory(): LettuceConnectionFactory =
        LettuceConnectionFactory(RedisURI.create(redisUri).toSpringConfig())

    @Bean
    fun cacheManager(connectionFactory: RedisConnectionFactory): RedisCacheManager {
        return RedisCacheManager.builder(connectionFactory)
            .cacheDefaults(RedisCacheConfiguration.defaultCacheConfig().entryTtl(Duration.ofDays(30)))
            .build()
    }
}

fun main(args: Array<String>) {
    runApplication<GearManagementApplication>(*args)
}
